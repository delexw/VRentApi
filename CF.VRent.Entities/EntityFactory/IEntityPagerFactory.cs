using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Entities.EntityFactory
{
    public interface IEntityPagerFactory<TEntity> where TEntity : RestfulCommonObject
    {
        EntityPager<TEntity> CreateEntity(IEnumerable<TEntity> roots, Pager pager);
    }
}
