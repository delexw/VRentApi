using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace CF.VRent.DataAccessProxy.Entities
{
    public enum IndirectFeeSourceType {BuiltIn = 0,WriteIn = 1};

    [DataContract]
    public class IndirectFeeType
    {
        //[ID] [int] NOT NULL,
        //[Type] [nvarchar](50) NOT NULL,
        //[Group] [nvarchar](50) NOT NULL,
        //[Note] [nvarchar](50) NOT NULL,
        //[State] [tinyint] NOT NULL,
        //[CreatedOn] [datetime] NOT NULL,
        //[CreatedBy] [uniqueidentifier] NOT NULL,
        //[ModifiedOn] [datetime] NULL,
        //[ModifiedBy] [uniqueidentifier] NULL,

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Group { get; set; }

        [DataMember]
        public IndirectFeeSourceType SourceType { get; set; }

        [DataMember]
        public string Note { get; set; }

        [DataMember]
        public int State { get; set; }

        [DataMember]
        public DateTime? CreatedOn { get; set; }
        [DataMember]
        public Guid? CreatedBy { get; set; }
        [DataMember]
        public DateTime? ModifiedOn { get; set; }
        [DataMember]
        public Guid? ModifiedBy { get; set; }
    }
}
