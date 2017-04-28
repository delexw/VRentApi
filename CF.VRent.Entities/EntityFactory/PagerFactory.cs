using CF.VRent.Common.Entities;
using CF.VRent.Common.Entities.Interface;
using CF.VRent.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CF.VRent.Entities.EntityFactory
{
    /// <summary>
    /// Create entity with pager info
    /// </summary>
    /// <typeparam name="TEnitity"></typeparam>
    public class PagerFactory<TEnitity> : IEntityPagerFactory<TEnitity> where TEnitity : RestfulCommonObject
    {
        public EntityPager<TEnitity> CreateEntity(IEnumerable<TEnitity> roots, Pager pager)
        {
            //Calculate total page
            if (pager.TotalCount > 0 && pager.ItemsPerPage > 0)
            {
                pager.TotalPage = Math.Ceiling((pager.TotalCount.ToDecimal() / pager.ItemsPerPage.ToDecimal()).ToDecimal()).ToLong();
            }

            return new EntityPager<TEnitity>() { 
                Enitites = roots,
                ItemsPerPage = pager.ItemsPerPage,
                PageNumber = pager.PageNumber,
                TotalPage = pager.TotalPage,
                TotalCount = pager.TotalCount
            };
        }
    }
}
