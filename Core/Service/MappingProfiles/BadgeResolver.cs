using AutoMapper;
using Domain.Entities.Groups;
using Shared.DTOs.LeaderboardModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.MappingProfiles
{
    public class BadgeResolver : IValueResolver<GroupScore, LeaderboardGroupDTO, string>
    {
        public string Resolve(GroupScore src, LeaderboardGroupDTO dest, string destMember, ResolutionContext ctx)
        {
            int points = src.TotalPoints;

            if (points >= 5000) return "Legend";
            if (points >= 2500) return "Diamond";
            if (points >= 1000) return "Gold";
            if (points >= 500) return "Silver";
            return "Bronze";
        }
    }
}
