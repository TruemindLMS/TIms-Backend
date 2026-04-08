using Microsoft.EntityFrameworkCore;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Infrastructure.DbContext;

namespace TeamIndia.TalentFlow.API.Helpers
{
    public static class TeamDataSeeder
    {
        public static async Task SeedTeamsAsync(ApplicationDbContext db)
        {
            if (db == null) return;

            if (await db.Teams.AnyAsync()) return;

            var teams = new List<Team>
            {
                new Team { TeamId = Guid.NewGuid(), Name = "Team Alpha" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Bravo" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Charlie" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Delta" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Echo" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Foxtrot" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Golf" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Hotel" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team India" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Juliet" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Kilo" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Lima" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Mike" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team November" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Oscar" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Papa" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Quebec" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Romeo" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Sierra" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Tango" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Uniform" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Victor" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Whiskey" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team X-ray" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Yankee" },
                new Team { TeamId = Guid.NewGuid(), Name = "Team Zulu" }
            };

            await db.Teams.AddRangeAsync(teams);
            await db.SaveChangesAsync();
        }
    }
}
