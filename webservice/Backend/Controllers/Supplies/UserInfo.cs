﻿using Backend.Controllers.Supplies.Filters;
using Backend.Controllers.Supplies.ViewModels;
using Backend.Models;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Controllers.Supplies
{
    public class UserInfo
    {
        private ModelDbContext _model = new ModelDbContext();
        public List<Claim> claims { get; set; }
        private List<WashViewModel> _washes;


        public UserInfo(List<Claim> c)
        {
            this.claims = c;
        }

        public List<WashViewModel> GetWashes()
        {
            List<WashViewModel> result = new List<WashViewModel>();

            foreach (Claim c in claims)
            {
                if (c.Type == "MSO")
                {
                    Wash w = _model.Wash.Where(w => w.Code == c.Value).FirstOrDefault();
                    result.Add(new WashViewModel()
                    {
                        idWash = w.Idwash,
                        code = w.Code,
                        name = w.Name,
                        idRegion = w.Idregion
                    });
                }
            }

            _washes = result;
            return result;
        }

        public List<PostViewModel> GetPosts()
        {
            if (_washes == null)
                this.GetWashes();
            List<PostViewModel> result = new List<PostViewModel>();

            foreach (WashViewModel w in _washes)
            {
                List<Posts> posts = _model.Posts.Where(p => p.Idwash.Equals(w.idWash)).Include(p => p.IddeviceNavigation).ToList();

                foreach (Posts p in posts)
                {
                    if (p.Iddevice == null)
                        continue;

                    result.Add(new PostViewModel()
                    {
                        code = p.IddeviceNavigation.Code,
                        name = p.IddeviceNavigation.Name,
                        idWash = w.idWash
                    });
                }
            }
            return result;
        }

        public List<RegionViewModel> GetRegions()
        {
            if (_washes == null)
                this.GetWashes();
            List<RegionViewModel> result = new List<RegionViewModel>();

            foreach (WashViewModel w in _washes)
            {
                Regions region = _model.Regions.Find(w.idRegion);

                RegionViewModel rvm = new RegionViewModel()
                {
                    idRegion = region.Idregion,
                    code = region.Code,
                    name = region.Name
                };

                if (result.Find(r => r.idRegion == rvm.idRegion) == null)
                    result.Add(rvm);
            }
            return result;
        }

        public List<ChangerViewModel> GetChangers()
        {
            if (_washes == null)
                this.GetWashes();

            List<ChangerViewModel> result = new List<ChangerViewModel>();

            foreach (WashViewModel w in _washes)
            {
                List<Changers> changers = _model.Changers.Where(c => c.Idwash == w.idWash).Include(c => c.IddeviceNavigation).ToList();

                foreach (Changers c in changers)
                {
                    if (c.Iddevice == null)
                        continue;

                    result.Add(new ChangerViewModel { code = c.IddeviceNavigation.Code, name = c.IddeviceNavigation.Name, idRegion = w.idRegion });
                }
            }

            return result;
        }

        public List<OperationTypeViewModel> GetOperationTypes()
        {
            List<OperationTypeViewModel> result = new List<OperationTypeViewModel>();

            List<OperationTypes> types = _model.OperationTypes.ToList();
            foreach(OperationTypes ot in types)
            {
                result.Add(new OperationTypeViewModel { code = ot.Code, name = ot.Name });
            }

            return result;
        }

        public List<EventChangerKindViewModel> GetEventChangerKinds()
        {
            List<EventChangerKindViewModel> result = new List<EventChangerKindViewModel>();

            List<EventChangerKind> kinds = _model.EventChangerKind.ToList();
            foreach(EventChangerKind eck in kinds)
            {
                result.Add(new EventChangerKindViewModel { code = eck.Code, name = eck.Name });
            }

            return result;
        }

        public List<GroupViewModel> GetGroups()
        {
            if (_washes == null)
                this.GetWashes();
            List<GroupViewModel> result = new List<GroupViewModel>();

            var name = this.claims.Where(c => c.Type == ClaimsIdentity.DefaultNameClaimType).FirstOrDefault().Value;

            //_model.WashGroup.Include(g => g.IdgroupNavigation).Include(w => w.IdwashNavigation);
            //_model.Users.Where(u => u.Login == this.claims.Where(c => c.Type == ClaimsIdentity.DefaultNameClaimType).FirstOrDefault().Value).Include(uw => );
            var x = _model.UserWash.Include(u => u.IduserNavigation).Where(u => u.IduserNavigation.Login == name)
                           .Include(w => w.IdwashNavigation).Join(_model.WashGroup.Include(g => g.IdgroupNavigation), w => w.IdwashNavigation.Idwash, wg => wg.Idwash,
                           (w, wg) => new GroupViewModel { name = wg.IdgroupNavigation.Name, code = wg.IdgroupNavigation.Code }).ToList();

            foreach(GroupViewModel g in x)
                if (result.Where(xx => xx.code == g.code).FirstOrDefault() == null)
                    result.Add(g);
                                     //.Join(_washes, wg => wg.IdwashNavigation.Idwash, w => w.idWash, 
                                     //(wg, w) => new GroupViewModel { 
                                     //    name = wg.IdgroupNavigation.Name, 
                                     //    code = wg.IdgroupNavigation.Code }).Select(g => new GroupViewModel { name = g.name, code = g.code }).ToList();
            return result;                            
        }
    }
}
