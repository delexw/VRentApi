using CF.VRent.SAPSDK.Entities;
using CF.VRent.SAPSDK.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CF.VRent.Common;

namespace CF.VRent.SAPSDK
{
    public class SAPManager : ISAPManager,IDisposable
    {
        public PostEntityCollection Common
        {
            get;
            private set;
        }

        public PostEntityCollection Header
        {
            get;
            private set;
        }

        public PostEntityCollection Item
        {
            get;
            private set;
        }

        public PostEntityCollection FileName
        {
            get;
            private set;
        }

        public SAPManager()
        {
            foreach (PostConfigurationEntity doc in SAPContext.Documents)
            {
                var d = doc.Doc;

                var nodes = d.SelectNodes("//Settings/Setting");

                List<PostEntity> entities = new List<PostEntity>();

                foreach (XmlNode node in nodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        if (entities.FirstOrDefault(r => r.Name.ToStr().Trim() == node.Attributes["name"].ToStr().Trim()) == null)
                        {
                            entities.Add(new PostEntity()
                            {
                                Name = node.Attributes["name"] == null ? null : node.Attributes["name"].Value,
                                Value = node.Attributes["value"] == null ? null : node.Attributes["value"].Value,
                                Order = node.Attributes["order"] == null ? 0 : node.Attributes["order"].Value.ToInt()
                            });
                        }
                        else
                        {
                            throw new Exception(node.Attributes["name"] + " is duplicate");
                        }
                    }
                }

                switch (doc.Type)
                {
                    case EntityType.Common:
                        Common = new PostEntityCollection(doc.Type, entities);
                        break;
                    case EntityType.FileName:
                        FileName = new PostEntityCollection(doc.Type, entities, "");
                        break;
                    case EntityType.Header:
                        Header = new PostEntityCollection(doc.Type, entities, Common[SAPSDKConstants.FieldDelimiter]);
                        break;
                    case EntityType.Item:
                        Item = new PostEntityCollection(doc.Type, entities, Common[SAPSDKConstants.FieldDelimiter]);
                        break;
                }
            }
        }

        public void Dispose()
        {
            this.Common = null;
            this.FileName = null;
            this.Header = null;
            this.Item = null;
        }
    }
}
