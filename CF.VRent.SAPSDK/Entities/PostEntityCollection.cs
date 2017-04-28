using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.SAPSDK.Entities
{
    public class PostEntityCollection : IEnumerable<PostEntity>
    {
        private List<PostEntity> _post;
        private EntityType _type;
        private string _seperator;

        public PostEntityCollection(EntityType type, IEnumerable<PostEntity> posts, string fieldsSeperator="|")
        {
            _post = posts.ToList();
            _type = type;
            _seperator = fieldsSeperator;
        }

        public string this[string key]
        {
            get
            {
                var item = _post.FirstOrDefault(r => r.Name == key.Trim());
                if (item == null)
                {
                    throw new NullReferenceException(key);
                }
                return item.Value;
            }
            set
            {
                var temp = _post.FirstOrDefault(r => r.Name == key.Trim());
                temp.Value = value;
            }
        }

        /// <summary>
        /// Get the formater string
        /// </summary>
        public string FormatEntity
        {
            get
            {
                var orderEntity = _post.OrderBy(r => r.Order);
                var str = "";
                foreach (PostEntity e in _post)
                {
                    str += e.Value + _seperator;
                }
                if (String.IsNullOrWhiteSpace(_seperator))
                {
                    //the last character is a null or empty string
                    //don't need to format it
                    return str;
                }
                else
                {
                    return str.Substring(0, str.Length - 1);
                }
            }
        }

        public IEnumerator<PostEntity> GetEnumerator()
        {
            return _post.AsEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _post.GetEnumerator();
        }
    }
}
