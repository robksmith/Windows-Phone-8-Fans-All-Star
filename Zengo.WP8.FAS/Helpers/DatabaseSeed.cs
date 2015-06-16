
#region Usings

using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.WebApi.Responses;
using System;
using System.Data.Linq;
using System.Linq;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public class DatabaseSeed
    {
        /// <summary>
        /// Given a batch of json files, create the database and populate the database
        /// </summary>
        public void CreateDatabaseFromInitialSeedData(FasDataContext db, BatchUpdate batch)
        {
            // Delete it just in case
            db.DeleteDatabase();

            // Create the local database.
            db.CreateDatabase();

            // Extract data from the json files
            PopulateDatabase(db, batch);

            // Save to the database.
            db.SubmitChanges();

            // mainly for info...
            int playersCount = (from item in db.Players select item).Count();
            int clubsCount = (from item in db.Clubs select item).Count();
            int leaguesCount = (from item in db.Leagues select item).Count();
            int countriesCount = (from item in db.Countries select item).Count();
            int continentsCount = (from item in db.Continents select item).Count();
            int zonesCount = (from item in db.Zones select item).Count();
            int positionsCount = (from item in db.Positions select item).Count();
            int packagesCount = (from item in db.Packages select item).Count();
        }


        /// <summary>
        /// Given the batch, populate the database
        /// </summary>
        private void PopulateDatabase(FasDataContext db, BatchUpdate batch)
        {
            // get all json entities
            var jsonPlayers = from player in batch.Players select player;
            var jsonClubs = from club in batch.Clubs select club;
            var jsonLeagues = from league in batch.Leagues select league;
            var jsonContinents = from continent in batch.Continents select continent;
            var jsonCountries = from country in batch.Countries select country;
            var jsonPositions = from position in batch.Positions select position;
            var jsonZones = from zone in batch.Zones select zone;
            var jsonPackages = from zone in batch.Packages select zone;

            // count the json entries (just for info)
            int jsonPlayersCount = jsonPlayers.Count();
            int jsonClubsCount = jsonClubs.Count();
            int jsonLeaguesCount = jsonLeagues.Count();
            int jsonContinentsCount = jsonContinents.Count();
            int jsonCountriesCount = jsonCountries.Count();
            int jsonPositionsCount = jsonPositions.Count();
            int jsonZonesCount = jsonZones.Count();
            int jsonPackagesCount = jsonPackages.Count();

            // Create the dummy records
            LeagueRecord nullLeagueRecord = new LeagueRecord
            {
                LeagueId = DatabaseHelper.DummyId,
                LastModified = DateTime.Now,
                Name = "None"
            };

            ClubRecord nullClubRecord = new ClubRecord
            {
                ClubId = DatabaseHelper.DummyId,
                Name = "None",
                LastModified = DateTime.Now,
                League = nullLeagueRecord
            };

            ContinentRecord nullContinentRecord = new ContinentRecord
            {
                ContinentId = DatabaseHelper.DummyId,
                Name = "None",
                LastModified = DateTime.Now
            };

            CountryRecord nullCountryRecord = new CountryRecord
            {
                CountryId = DatabaseHelper.DummyId,
                Name = "None",
                LastModified = DateTime.Now,

                Continent = nullContinentRecord
            };

            ZoneRecord nullZoneRecord = new ZoneRecord
            {
                ZoneId = DatabaseHelper.DummyId,
                Name = "None",
                LastModified = DateTime.Now
            };

            PlayerRecord nullPlayerRecord = new PlayerRecord
            {
                PlayerId = DatabaseHelper.DummyId,
                LastName = "None",
                LastModified = DateTime.Now,

                Club = nullClubRecord,
                Country = nullCountryRecord,
                Zone = nullZoneRecord
            };

            PositionRecord nullPositionRecord = new PositionRecord
            {
                PositionId = DatabaseHelper.DummyId,
                Name = "None",
                LastModified = DateTime.Now,
                
                Zone = nullZoneRecord
            };

            PackageRecord nullPackagesRecord = new PackageRecord
            {
                PackageId = DatabaseHelper.DummyId,
                Name = "None",
                LastModified = DateTime.Now
            };

            // Insert the dummy records
            db.Leagues.InsertOnSubmit(nullLeagueRecord);
            db.Clubs.InsertOnSubmit(nullClubRecord);
            db.Continents.InsertOnSubmit(nullContinentRecord);
            db.Countries.InsertOnSubmit(nullCountryRecord);
            db.Zones.InsertOnSubmit(nullZoneRecord);
            db.Players.InsertOnSubmit(nullPlayerRecord);
            db.Positions.InsertOnSubmit(nullPositionRecord);
            db.Packages.InsertOnSubmit(nullPackagesRecord);


            // Create entity sets
            var leaguesEntities = new EntitySet<LeagueRecord>();
            var clubsEntities = new EntitySet<ClubRecord>();
            var continentsEntities = new EntitySet<ContinentRecord>();
            var countriesEntities = new EntitySet<CountryRecord>();
            var zonesEntities = new EntitySet<ZoneRecord>();
            var playerEntities = new EntitySet<PlayerRecord>();
            var positionsEntities = new EntitySet<PositionRecord>();
            var packagesEntities = new EntitySet<PackageRecord>();


            // for all leagues, create an entity
            foreach (League leagueJson in jsonLeagues)
            {
                // create a new league record
                LeagueRecord newLeagueRecord = new LeagueRecord
                {
                    LeagueId = leagueJson.Id,
                    Name = leagueJson.Name,
                    LastModified = DateTime.Now
                };
                leaguesEntities.Add(newLeagueRecord);
            }
            db.Leagues.InsertAllOnSubmit(leaguesEntities);
            db.SubmitChanges();


            // for all clubs, create an entity
            foreach (Club clubJson in jsonClubs)
            {
                // create a new club record
                ClubRecord newClubRecord = new ClubRecord
                {
                    ClubId = clubJson.Id,
                    Name = clubJson.Name,
                    Country = clubJson.Country,
                    Image = clubJson.Image,
                    LastModified = DateTime.Now,

                    League = nullLeagueRecord
                };
                clubsEntities.Add(newClubRecord);
            }
            db.Clubs.InsertAllOnSubmit(clubsEntities);
            db.SubmitChanges();


            // for all continents, create an entity
            foreach (Continent continentJson in jsonContinents)
            {
                // create a new continent record
                ContinentRecord newContinentRecord = new ContinentRecord
                {
                    ContinentId = continentJson.Id,
                    Name = continentJson.Name,
                    LastModified = DateTime.Now
                };
                continentsEntities.Add(newContinentRecord);
            }
            db.Continents.InsertAllOnSubmit(continentsEntities);
            db.SubmitChanges();


            // for all countries, create an entity
            foreach (Country countryJson in jsonCountries)
            {
                // create a new country record
                CountryRecord newCountryRecord = new CountryRecord
                {
                    CountryId = countryJson.Id,
                    Name = countryJson.Name,
                    IsEurope = countryJson.IsEurope,
                    Selectable = countryJson.Selectable,
                    ShortName = countryJson.ShortName,
                    Telephone = countryJson.Telephone,
                    CallCost = countryJson.CallCost,
                    Sms = countryJson.Sms,
                    SmsCost = countryJson.SmsCost,
                    Image = countryJson.Image,
                    LastModified = DateTime.Now,

                    Continent = nullContinentRecord
                };
                countriesEntities.Add(newCountryRecord);
            }
            db.Countries.InsertAllOnSubmit(countriesEntities);
            db.SubmitChanges();


            // for all zones, create an entity
            foreach (Zone zoneJson in jsonZones)
            {
                // create a new continent record
                ZoneRecord newZoneRecord = new ZoneRecord
                {
                    ZoneId = zoneJson.Id,
                    Name = zoneJson.Name,
                    Group = zoneJson.Group,
                    LastModified = DateTime.Now
                };
                zonesEntities.Add(newZoneRecord);
            }
            db.Zones.InsertAllOnSubmit(zonesEntities);
            db.SubmitChanges();


            // for all players, create an entity
            foreach (Player playerJson in jsonPlayers)
            {
                // create a new player record
                PlayerRecord newPlayerRecord = new PlayerRecord()
                {
                    PlayerId = playerJson.Id,
                    FirstName = playerJson.Name,
                    LastName = playerJson.Surname,
                    Image = playerJson.Image,
                    IsCulled = playerJson.Culled,
                    IsDeleted = playerJson.Deleted,
                    IsEurope = playerJson.Europe,
                    Moving = playerJson.Moving,
                    LastModified = DateTime.Now,

                    Club = nullClubRecord,
                    Country = nullCountryRecord,
                    Zone = nullZoneRecord
                };
                playerEntities.Add(newPlayerRecord);
            }
            db.Players.InsertAllOnSubmit(playerEntities);
            db.SubmitChanges();


            // for all positions, create an entity
            foreach (Position positionJson in jsonPositions)
            {
                // create a new continent record
                PositionRecord newPositionRecord = new PositionRecord
                {
                    PositionId = positionJson.Id,
                    Name = positionJson.Name,
                    Key = positionJson.Key,
                    Group = positionJson.Group,
                    IsEurope = positionJson.IsEurope,
                    LastModified = DateTime.Now,

                    Zone = nullZoneRecord
                };
                positionsEntities.Add(newPositionRecord);
            }
            db.Positions.InsertAllOnSubmit(positionsEntities);
            db.SubmitChanges();


            // for all packages, create an entity
            foreach (Package packageJson in jsonPackages)
            {
                // create a new continent record
                PackageRecord newPackageRecord = new PackageRecord
                {
                    PackageId = packageJson.Id,
                    Name = packageJson.Name,
                    Bid = packageJson.Bid,
                    Price = packageJson.Price,
                    Votes = packageJson.Votes,
                    LastModified = DateTime.Now
                };
                packagesEntities.Add(newPackageRecord);
            }
            db.Packages.InsertAllOnSubmit(packagesEntities);
            db.SubmitChanges();

            // Now link up all of these records
            SeedLinkUp(db, jsonClubs, jsonCountries, jsonPlayers, jsonPositions, clubsEntities, countriesEntities, playerEntities, positionsEntities);
        }



        /// <summary>
        /// Given the newly created entities above, link the database up
        /// </summary>
        private static void SeedLinkUp(FasDataContext db,
            System.Collections.Generic.IEnumerable<Club> jsonClubs,
            System.Collections.Generic.IEnumerable<Country> jsonCountries,
            System.Collections.Generic.IEnumerable<Player> jsonPlayers,
            System.Collections.Generic.IEnumerable<Position> jsonPositions,
            EntitySet<ClubRecord> clubsEntities,
            EntitySet<CountryRecord> countriesEntities,
            EntitySet<PlayerRecord> playerEntities,
            EntitySet<PositionRecord> positionsEntities
            )
        {
            // now for each newly created club above, link it up
            foreach (ClubRecord newClub in clubsEntities)
            {
                // get this clubs json
                Club jsonClub = (from club in jsonClubs where club.Id == newClub.ClubId select club).First();

                // get this clubs league
                LeagueRecord leagueRecord = (from league in db.Leagues where league.LeagueId == jsonClub.LeagueId select league).FirstOrDefault();

                // link up
                if (leagueRecord != null) newClub.League = leagueRecord;
            }

            // submit the position changes
            db.SubmitChanges();


            // now for each newly created country above, link it up
            foreach (CountryRecord newCountry in countriesEntities)
            {
                // get this countries json
                Country jsonCountry = (from country in jsonCountries where country.Id == newCountry.CountryId select country).First();

                // get this countries continent
                ContinentRecord continentRecord = (from continent in db.Continents where continent.ContinentId == jsonCountry.ContinentId select continent).FirstOrDefault();

                // link up
                if (continentRecord != null) newCountry.Continent = continentRecord;
            }

            // submit the position changes
            db.SubmitChanges();


            // now for each newly created player above, link it up
            foreach (PlayerRecord newPlayer in playerEntities)
            {
                // get this players json
                Player jsonPlayer = (from player in jsonPlayers where player.Id == newPlayer.PlayerId select player).First();

                // get this players club
                ClubRecord clubRecord = (from club in db.Clubs where club.ClubId == jsonPlayer.ClubId select club).FirstOrDefault();
                if (clubRecord != null) newPlayer.Club = clubRecord;

                // get this players country
                CountryRecord countryRecord = (from country in db.Countries where country.CountryId == jsonPlayer.CountryId select country).FirstOrDefault();
                if (countryRecord != null) newPlayer.Country = countryRecord;

                // get this players zone
                ZoneRecord zoneRecord = (from zone in db.Zones where zone.ZoneId == jsonPlayer.ZoneId select zone).FirstOrDefault();
                if (zoneRecord != null) newPlayer.Zone = zoneRecord;
            }

            // submit the position changes
            db.SubmitChanges();


            // now for each newly created position above, link it up
            foreach (PositionRecord newPosition in positionsEntities)
            {
                // get this position json
                Position jsonPosition = (from position in jsonPositions where position.Id == newPosition.PositionId select position).First();

                // get this positions zone
                ZoneRecord zoneRecord = (from zone in db.Zones where zone.ZoneId == jsonPosition.ZoneId select zone).FirstOrDefault();

                // link up
                if (zoneRecord != null) newPosition.Zone = zoneRecord;
            }

            // submit the position changes
            db.SubmitChanges();



            // Pre evaluate club player count
            foreach (ClubRecord newClub in clubsEntities)
            {
                newClub.PlayerCount = newClub.Players.Count();
            }

            // Pre evaluate country player count
            foreach (CountryRecord newCountry in countriesEntities)
            {
                newCountry.PlayerCount = newCountry.Players.Count();
            }

            // submit the position changes
            db.SubmitChanges();
        }

    }

}