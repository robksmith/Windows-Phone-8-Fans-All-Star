
#region Usings

using Zengo.WP8.FAS.Models;
using Zengo.WP8.FAS.WebApi.Responses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;

#endregion

namespace Zengo.WP8.FAS.Helpers
{
    public class DatabaseUpdate
    {
        Dictionary<int, PlayerRecord> playerDictionary;
        Dictionary<int, ClubRecord> clubDictionary;
        Dictionary<int, LeagueRecord> leagueDictionary;
        Dictionary<int, CountryRecord> countryDictionary;
        Dictionary<int, ContinentRecord> continentDictionary;
        Dictionary<int, ZoneRecord> zoneDictionary;
        Dictionary<int, PositionRecord> positionDictionary;
        Dictionary<int, PackageRecord> packageDictionary;

        public DatabaseUpdate()
        {
            // Add items to a dictionary for rapid finding

            playerDictionary = new Dictionary<int, PlayerRecord>();
            foreach (PlayerRecord player in App.ViewModel.DbViewModel.AllPlayers)
            {
                playerDictionary.Add(player.PlayerId, player);
            }

            clubDictionary = new Dictionary<int, ClubRecord>();
            foreach (ClubRecord club in App.ViewModel.DbViewModel.AllClubs)
            {
                clubDictionary.Add(club.ClubId, club);
            }

            leagueDictionary = new Dictionary<int, LeagueRecord>();
            foreach (LeagueRecord league in App.ViewModel.DbViewModel.AllLeagues)
            {
                leagueDictionary.Add(league.LeagueId, league);
            }

            countryDictionary = new Dictionary<int, CountryRecord>();
            foreach (CountryRecord country in App.ViewModel.DbViewModel.AllCountries)
            {
                countryDictionary.Add(country.CountryId, country);
            }

            continentDictionary = new Dictionary<int, ContinentRecord>();
            foreach (ContinentRecord continent in App.ViewModel.DbViewModel.AllContinents)
            {
                continentDictionary.Add(continent.ContinentId, continent);
            }

            zoneDictionary = new Dictionary<int, ZoneRecord>();
            foreach (ZoneRecord zone in App.ViewModel.DbViewModel.AllZones)
            {
                zoneDictionary.Add(zone.ZoneId, zone);
            }

            positionDictionary = new Dictionary<int, PositionRecord>();
            foreach (PositionRecord position in App.ViewModel.DbViewModel.AllPositions)
            {
                positionDictionary.Add(position.PositionId, position);
            }

            packageDictionary = new Dictionary<int, PackageRecord>();
            foreach (PackageRecord package in App.ViewModel.DbViewModel.AllPackages)
            {
                packageDictionary.Add(package.PackageId, package);
            }
        }


        /// <summary>
        /// Given a batch of json updates, update the database
        /// </summary>
        internal void ApplyUpdates(FasDataContext db, BatchUpdate batch, BackgroundWorker worker)
        {
            // Extract data from the json files
            UpdateDatabase(db, batch, worker);

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
        /// Tell the background worker how much work we have completed 
        /// </summary>
        private static int ReportProgress(BackgroundWorker worker, int count, int total)
        {
            int percent = (int)((float)count / (float)total * 100f);

            worker.ReportProgress(percent);

            return count + 1;
        }


        /// <summary>
        /// ******* Apply a set of updates to the database *********
        /// </summary>
        private void UpdateDatabase(FasDataContext db, BatchUpdate batch, BackgroundWorker worker)
        {
            // Count the total number or records to be processed
            int total = batch.TotalRecords();

            // Add the total for the link up phase
            if (batch.Clubs != null) total += batch.Clubs.Count();
            if (batch.Countries != null) total += batch.Countries.Count();
            if (batch.Players != null) total += batch.Players.Count();
            if (batch.Positions != null) total += batch.Positions.Count();

            // Add extra because we do more processing with the club and country
            if (batch.Clubs != null) total += batch.Clubs.Count();
            if (batch.Countries != null) total += batch.Countries.Count();

            // reset the counter
            int count = 0;

            // get all json entities
            //var jsonPlayers = from player in batch.Players select player;
            //var jsonClubs = from club in batch.Clubs select club;
            //var jsonLeagues = from league in batch.Leagues select league;
            //var jsonContinents = from continent in batch.Continents select continent;
            //var jsonCountries = from country in batch.Countries select country;
            //var jsonPositions = from position in batch.Positions select position;
            //var jsonZones = from zone in batch.Zones select zone;
            //var jsonPackages = from zone in batch.Packages select zone;

            //// count the json entries
            //int jsonPlayersCount = jsonPlayers.Count();
            //int jsonClubsCount = jsonClubs.Count();
            //int jsonLeaguesCount = jsonLeagues.Count();
            //int jsonContinentsCount = jsonContinents.Count();
            //int jsonCountriesCount = jsonCountries.Count();
            //int jsonPositionsCount = jsonPositions.Count();
            //int jsonZonesCount = jsonZones.Count();
            //int jsonPackagesCount = jsonPackages.Count();

            // get the dummy records
            LeagueRecord nullLeagueRecord = (from league in App.ViewModel.DbViewModel.AllLeagues where league.LeagueId == DatabaseHelper.DummyId select league).FirstOrDefault();
            ClubRecord nullClubRecord = (from club in App.ViewModel.DbViewModel.AllClubs where club.ClubId == DatabaseHelper.DummyId select club).FirstOrDefault();
            ContinentRecord nullContinentRecord = (from continent in App.ViewModel.DbViewModel.AllContinents where continent.ContinentId == DatabaseHelper.DummyId select continent).FirstOrDefault();
            CountryRecord nullCountryRecord = (from country in App.ViewModel.DbViewModel.AllCountries where country.CountryId == DatabaseHelper.DummyId select country).FirstOrDefault();
            ZoneRecord nullZoneRecord = (from zone in App.ViewModel.DbViewModel.AllZones where zone.ZoneId == DatabaseHelper.DummyId select zone).FirstOrDefault();
            PlayerRecord nullPlayerRecord = (from player in App.ViewModel.DbViewModel.AllPlayers where player.PlayerId == DatabaseHelper.DummyId select player).FirstOrDefault();
            PositionRecord nullPositionRecord = (from position in App.ViewModel.DbViewModel.AllPositions where position.PositionId == DatabaseHelper.DummyId select position).FirstOrDefault();
            PackageRecord nullPackageRecord = (from package in App.ViewModel.DbViewModel.AllPackages where package.PackageId == DatabaseHelper.DummyId select package).FirstOrDefault();


            // for all leagues to be updated
            if (batch.Leagues != null)
            {
                var leaguesEntities = new EntitySet<LeagueRecord>();
                foreach (League leagueJson in batch.Leagues)
                {
                    //// Find this league record in the db
                    //LeagueRecord leagueRecord = (from leagues in App.ViewModel.DbViewModel.AllLeagues
                    //                             where leagues.LeagueId == leagueJson.Id
                    //                             select leagues).FirstOrDefault();

                    //// check if it exists
                    //if (leagueRecord == null)
                    if (!leagueDictionary.ContainsKey(leagueJson.Id))
                    {
                        // if it doesnt, create one
                        LeagueRecord newLeagueRecord = new LeagueRecord
                        {
                            LeagueId = leagueJson.Id,
                            Name = leagueJson.Name,
                            LastModified = DateTime.Now,
                        };
                        leaguesEntities.Add(newLeagueRecord);
                        App.ViewModel.DbViewModel.AllLeagues.Add(newLeagueRecord);
                        leagueDictionary.Add(newLeagueRecord.LeagueId, newLeagueRecord); 
                    }
                    else
                    {
                        // Get the league
                        LeagueRecord leagueRecord = leagueDictionary[leagueJson.Id]; 
                 
                        // if it does, just update the existing record
                        leagueRecord.Name = leagueJson.Name;
                        leagueRecord.LastModified = DateTime.Now;
                    }

                    // Report progress to the worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the league changes
                db.Leagues.InsertAllOnSubmit(leaguesEntities);
                db.SubmitChanges();
            }


            // for all clubs to be updated
            var clubsEntities = new EntitySet<ClubRecord>();
            if (batch.Clubs != null)
            {
                foreach (Club clubJson in batch.Clubs)
                {
                    //// Find this club record in the db
                    //ClubRecord clubRecord = (from club in App.ViewModel.DbViewModel.AllClubs
                    //                         where club.ClubId == clubJson.Id
                    //                         select club).FirstOrDefault();

                    //// check if it exists
                    //if (clubRecord == null)
                    if (!clubDictionary.ContainsKey(clubJson.Id))
                    {
                        // if it doesnt, create one
                        ClubRecord newClubRecord = new ClubRecord
                        {
                            ClubId = clubJson.Id,
                            Name = clubJson.Name,
                            Country = clubJson.Country,
                            Image = clubJson.Image,

                            League = nullLeagueRecord,
                            LastModified = DateTime.Now
                        };
                        clubsEntities.Add(newClubRecord);
                        App.ViewModel.DbViewModel.AllClubs.Add(newClubRecord);
                        clubDictionary.Add(newClubRecord.ClubId, newClubRecord);
                    }
                    else
                    {
                        // Get the club
                        ClubRecord clubRecord = clubDictionary[clubJson.Id]; 

                        // if it does, just update the existing record
                        clubRecord.Name = clubJson.Name;
                        clubRecord.Country = clubJson.Country;
                        clubRecord.Image = clubJson.Image;
                        clubRecord.LastModified = DateTime.Now;
                    }

                    // Report progress to teh worker
                    count = ReportProgress(worker, count, total);
                }

                db.Clubs.InsertAllOnSubmit(clubsEntities);
                db.SubmitChanges();
            }


            // for all continents
            if (batch.Continents != null)
            {
                var continentsEntities = new EntitySet<ContinentRecord>();
                foreach (Continent continentJson in batch.Continents)
                {
                    //// Find this continents record in the existing db
                    //ContinentRecord continentRecord = (from continent in db.Continents
                    //                                   where continent.ContinentId == continentJson.Id
                    //                                   select continent).FirstOrDefault();
                    //// check if it exists
                    //if (continentRecord == null)
                    if (!continentDictionary.ContainsKey(continentJson.Id))
                    {
                        // create a new continent record
                        ContinentRecord newContinentRecord = new ContinentRecord
                        {
                            ContinentId = continentJson.Id,
                            Name = continentJson.Name,
                            LastModified = DateTime.Now
                        };
                        continentsEntities.Add(newContinentRecord);
                        App.ViewModel.DbViewModel.AllContinents.Add(newContinentRecord);
                        continentDictionary.Add(newContinentRecord.ContinentId, newContinentRecord);
                    }
                    else
                    {
                        // Get the continent
                        ContinentRecord continentRecord = continentDictionary[continentJson.Id]; 

                        // if it does, just update the existing record
                        continentRecord.Name = continentJson.Name;
                        continentRecord.LastModified = DateTime.Now;
                    }

                    // Report progress to teh worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the continent changes
                db.Continents.InsertAllOnSubmit(continentsEntities);
                db.SubmitChanges();
            }


            // for all countries
            var countriesEntities = new EntitySet<CountryRecord>();
            if (batch.Countries != null)
            {
                foreach (Country countryJson in batch.Countries)
                {
                    // Find this country record in the existing db
                    //CountryRecord countryRecord = (from country in App.ViewModel.DbViewModel.AllCountries
                    //                               where country.CountryId == countryJson.Id
                    //                               select country).FirstOrDefault();
                    // check if it exists
                    //if (countryRecord == null)
                    if (!countryDictionary.ContainsKey(countryJson.Id))
                    {
                        // create a new country record if it doesnt exist
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
                        App.ViewModel.DbViewModel.AllCountries.Add(newCountryRecord);
                        countryDictionary.Add(newCountryRecord.CountryId, newCountryRecord);
                    }
                    else
                    {
                        // Get the country
                        CountryRecord countryRecord = countryDictionary[countryJson.Id]; 

                        // if it does, just update the existing record
                        countryRecord.Name = countryJson.Name;
                        countryRecord.IsEurope = countryJson.IsEurope;
                        countryRecord.Selectable = countryJson.Selectable;
                        countryRecord.ShortName = countryJson.ShortName;
                        countryRecord.Telephone = countryJson.Telephone;
                        countryRecord.CallCost = countryJson.CallCost;
                        countryRecord.Sms = countryJson.Sms;
                        countryRecord.SmsCost = countryJson.SmsCost;
                        countryRecord.Image = countryJson.Image;
                        countryRecord.LastModified = DateTime.Now;
                    }

                    // Report progress to teh worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the country changes
                db.Countries.InsertAllOnSubmit(countriesEntities);
                db.SubmitChanges();
            }


            // for all zones
            if (batch.Zones != null)
            {
                var zonesEntities = new EntitySet<ZoneRecord>();
                foreach (Zone zoneJson in batch.Zones)
                {
                    //// Find this zone record in the existing db
                    //ZoneRecord zoneRecord = (from zone in App.ViewModel.DbViewModel.AllZones
                    //                         where zone.ZoneId == zoneJson.Id
                    //                         select zone).FirstOrDefault();
                    //// check if it exists
                    //if (zoneRecord == null)
                    if (!zoneDictionary.ContainsKey(zoneJson.Id))    
                    {
                        // create a new zone record
                        ZoneRecord newZoneRecord = new ZoneRecord
                        {
                            ZoneId = zoneJson.Id,
                            Name = zoneJson.Name,
                            Group = zoneJson.Group,
                            LastModified = DateTime.Now
                        };
                        zonesEntities.Add(newZoneRecord);
                        App.ViewModel.DbViewModel.AllZones.Add(newZoneRecord);
                        zoneDictionary.Add(newZoneRecord.ZoneId, newZoneRecord);
                    }
                    else
                    {
                        // Get hte zone
                        ZoneRecord zoneRecord = zoneDictionary[zoneJson.Id];   

                        // if it does, just update the existing record
                        zoneRecord.Name = zoneJson.Name;
                        zoneRecord.Group = zoneJson.Group;
                        zoneRecord.LastModified = DateTime.Now;
                    }

                    // Report progress to teh worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the zones changes
                db.Zones.InsertAllOnSubmit(zonesEntities);
                db.SubmitChanges();
            }


            // for all players
            var playerEntities = new EntitySet<PlayerRecord>();
            if (batch.Players != null)
            {
                foreach (Player playerJson in batch.Players)
                {
                    // Find this player record in the existing db
                    //PlayerRecord playerRecord = (from player in App.ViewModel.DbViewModel.AllPlayers
                    //                             where player.PlayerId == playerJson.Id
                    //                             select player).FirstOrDefault();

                    // check if it exists
                    //if (playerRecord == null)
                    if (!playerDictionary.ContainsKey(playerJson.Id))                               // XXX
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
                        App.ViewModel.DbViewModel.AllPlayers.Add(newPlayerRecord);
                        playerDictionary.Add(newPlayerRecord.PlayerId, newPlayerRecord);            // XXX
                    }
                    else
                    {
                        // Get the player
                        PlayerRecord playerRecord = playerDictionary[playerJson.Id];                // XXX

                        // if it does, just update the existing record
                        playerRecord.FirstName = playerJson.Name;
                        playerRecord.LastName = playerJson.Surname;
                        playerRecord.Image = playerJson.Image;
                        playerRecord.IsCulled = playerJson.Culled;
                        playerRecord.IsDeleted = playerJson.Deleted;
                        playerRecord.IsEurope = playerJson.Europe;
                        playerRecord.Moving = playerJson.Moving;
                        playerRecord.LastModified = DateTime.Now;
                    }

                    // Report progress to teh worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the players changes
                db.Players.InsertAllOnSubmit(playerEntities);
                db.SubmitChanges();
            }


            // for all positions
            var positionsEntities = new EntitySet<PositionRecord>();
            if (batch.Positions != null)
            {
                foreach (Position positionJson in batch.Positions)
                {
                    //// Find this position record in the existing db
                    //PositionRecord positionRecord = (from position in App.ViewModel.DbViewModel.AllPositions
                    //                                 where position.PositionId == positionJson.Id
                    //                                 select position).FirstOrDefault();

                    //// check if it exists
                    //if (positionRecord == null)
                    if (!positionDictionary.ContainsKey(positionJson.Id)) 
                    {
                        // create a new position record
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
                        App.ViewModel.DbViewModel.AllPositions.Add(newPositionRecord);
                        positionDictionary.Add(newPositionRecord.PositionId, newPositionRecord);
                    }
                    else
                    {
                        // Get the position
                        PositionRecord positionRecord = positionDictionary[positionJson.Id]; 

                        // if it does, just update the existing record
                        positionRecord.Name = positionJson.Name;
                        positionRecord.Key = positionJson.Key;
                        positionRecord.Group = positionJson.Group;
                        positionRecord.IsEurope = positionJson.IsEurope;
                        positionRecord.LastModified = DateTime.Now;
                    }

                    // Report progress to teh worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the position changes
                db.Positions.InsertAllOnSubmit(positionsEntities);
                db.SubmitChanges();
            }


            // for all packages
            if (batch.Packages != null)
            {
                var packagesEntities = new EntitySet<PackageRecord>();
                foreach (Package packageJson in batch.Packages)
                {
                    //// Find this package record in the existing db
                    //PackageRecord packageRecord = (from package in App.ViewModel.DbViewModel.AllPackages
                    //                               where package.PackageId == packageJson.Id
                    //                               select package).FirstOrDefault();

                    //// check if it exists
                    //if (packageRecord == null)
                    if (!packageDictionary.ContainsKey(packageJson.Id))
                    {
                        // create a new package record
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
                        App.ViewModel.DbViewModel.AllPackages.Add(newPackageRecord);
                        packageDictionary.Add(newPackageRecord.PackageId, newPackageRecord);
                    }
                    else
                    {
                        // Get the package
                        PackageRecord packageRecord = packageDictionary[packageJson.Id]; 

                        // if it does, just update the existing record
                        packageRecord.Name = packageJson.Name;
                        packageRecord.Bid = packageJson.Bid;
                        packageRecord.Price = packageJson.Price;
                        packageRecord.Votes = packageJson.Votes;
                        packageRecord.LastModified = DateTime.Now;
                    }

                    // Report progress to the worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the position changes
                db.Packages.InsertAllOnSubmit(packagesEntities);
                db.SubmitChanges();
            }

            //LinkUpOnlyNewRecords(worker, db, batch.Clubs, batch.Countries, batch.Players, batch.Positions, clubsEntities, countriesEntities, playerEntities, positionsEntities);

            // Link up every record in the db
            LinkUpAllRecords(worker, count, total, db, batch.Clubs, batch.Countries, batch.Players, batch.Positions, clubsEntities, countriesEntities, playerEntities, positionsEntities);
        }



        /// <summary>
        /// Given the newly created entities above, link the database up
        /// </summary>
        private void LinkUpAllRecords(BackgroundWorker worker, int count, int total, FasDataContext db,
            IEnumerable<Club> jsonClubs,
            IEnumerable<Country> jsonCountries,
            IEnumerable<Player> jsonPlayers,
            IEnumerable<Position> jsonPositions,
            EntitySet<ClubRecord> clubsEntities,
            EntitySet<CountryRecord> countriesEntities,
            EntitySet<PlayerRecord> playerEntities,
            EntitySet<PositionRecord> positionsEntities
            )
        {
            // now for EVERY club in the batch, link it up
            if (jsonClubs != null)
            {
                foreach (Club jsonClub in jsonClubs)
                {
                    // get this clubs record from the db
                    ClubRecord clubRecord = (from club in App.ViewModel.DbViewModel.AllClubs where club.ClubId == jsonClub.Id select club).First();
                    //ClubRecord clubRecord = clubDictionary[jsonClub.Id];

                    // get this clubs league
                    LeagueRecord leagueRecord = (from league in App.ViewModel.DbViewModel.AllLeagues where league.LeagueId == jsonClub.LeagueId select league).FirstOrDefault();
                    //LeagueRecord leagueRecord = leagueDictionary[jsonClub.LeagueId];

                    // Link up
                    if (leagueRecord != null) clubRecord.League = leagueRecord;

                    // Report progress to the worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the changes
                //db.SubmitChanges();
            }

            // now for EVERY country in the batch above, link it up
            if (jsonCountries != null)
            {
                foreach (Country jsonCountry in jsonCountries)
                {
                    // get this countries record from the db
                    CountryRecord countryRecord = (from country in App.ViewModel.DbViewModel.AllCountries where country.CountryId == jsonCountry.Id select country).First();
                    //CountryRecord countryRecord = countryDictionary[jsonCountry.Id];

                    // get this countries continent
                    ContinentRecord continentRecord = (from continent in App.ViewModel.DbViewModel.AllContinents where continent.ContinentId == jsonCountry.ContinentId select continent).FirstOrDefault();
                    //ContinentRecord continentRecord = continentDictionary[jsonCountry.ContinentId];

                    // Link up
                    if (continentRecord != null) countryRecord.Continent = continentRecord;

                    // Report progress to the worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the changes
                //db.SubmitChanges();
            }

            // now for EVERY player in the batch, link it up
            if (jsonPlayers != null)
            {
                //CountryRecord countryRecord = null;

                foreach (Player jsonPlayer in jsonPlayers)
                {

                    // get this players record
                    PlayerRecord playerRecord = (from player in App.ViewModel.DbViewModel.AllPlayers where player.PlayerId == jsonPlayer.Id select player).First();
                    //PlayerRecord playerRecord = playerDictionary[jsonPlayer.Id];

                    // get this players club
                    ClubRecord clubRecord = (from club in App.ViewModel.DbViewModel.AllClubs where club.ClubId == jsonPlayer.ClubId select club).FirstOrDefault();
                    //ClubRecord clubRecord = clubDictionary[jsonPlayer.ClubId];

                    // get this players country
                    CountryRecord countryRecord = (from country in App.ViewModel.DbViewModel.AllCountries where country.CountryId == jsonPlayer.CountryId select country).FirstOrDefault();
                    //countryDictionary.TryGetValue(jsonPlayer.CountryId, out countryRecord);


                    // get this players zone
                    ZoneRecord zoneRecord = (from zone in App.ViewModel.DbViewModel.AllZones where zone.ZoneId == jsonPlayer.ZoneId select zone).FirstOrDefault();
                    //ZoneRecord zoneRecord = zoneDictionary[jsonPlayer.ZoneId];


                    if (clubRecord != null) playerRecord.Club = clubRecord;
                    if (countryRecord != null) playerRecord.Country = countryRecord;
                    if (zoneRecord != null) playerRecord.Zone = zoneRecord;


                    // Report progress to the worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the changes
                //db.SubmitChanges();
            }

            // now for EVERY position in the batch above, link it up
            if (jsonPositions != null)
            {
                foreach (Position jsonPosition in jsonPositions)
                {
                    // get this positions record from the db
                    PositionRecord positionRecord = (from position in App.ViewModel.DbViewModel.AllPositions where position.PositionId == jsonPosition.Id select position).First();
                    //PositionRecord positionRecord = positionDictionary[jsonPosition.Id];

                    // get this positions zone
                    ZoneRecord zoneRecord = (from zone in App.ViewModel.DbViewModel.AllZones where zone.ZoneId == jsonPosition.ZoneId select zone).FirstOrDefault();
                    //ZoneRecord zoneRecord = zoneDictionary[jsonPosition.ZoneId];

                    // link up
                    if (zoneRecord != null) positionRecord.Zone = zoneRecord;

                    // Report progress to the worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the changes
                //db.SubmitChanges();
            }




            // Pre evaluate clubs player count
            if (jsonClubs != null)
            {
                foreach (Club jsonClub in jsonClubs)
                {
                    // get this clubs record from the db
                    ClubRecord clubRecord = (from club in App.ViewModel.DbViewModel.AllClubs where club.ClubId == jsonClub.Id select club).First();
                    clubRecord.PlayerCount = clubRecord.Players.Count();

                    // Report progress to the worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the changes
                //db.SubmitChanges();
            }

            // Pre evaluate country player count
            if (jsonCountries != null)
            {
                foreach (Country jsonCountry in jsonCountries)
                {
                    // get this countries record from the db
                    CountryRecord countryRecord = (from country in App.ViewModel.DbViewModel.AllCountries where country.CountryId == jsonCountry.Id select country).First();
                    countryRecord.PlayerCount = countryRecord.Players.Count();

                    // Report progress to the worker
                    count = ReportProgress(worker, count, total);
                }

                // submit the changes
                //db.SubmitChanges();
            }


            // submit the changes
            db.SubmitChanges();
        }

/*
        /// <summary>
        /// Given the newly created entities above, link the database up
        /// </summary>
        private static void LinkUpOnlyNewRecords(BackgroundWorker worker, FasDataContext db,
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
            int total = 0;
            if (jsonClubs != null) total += jsonClubs.Count();
            if (jsonCountries != null) total += jsonCountries.Count();
            if (jsonPlayers != null) total += jsonPlayers.Count();
            if (jsonPositions != null) total += jsonPositions.Count();

            int count = 0;

            // now for each newly created club above, link it up
            foreach (ClubRecord newClub in clubsEntities)
            {
                // get this club json
                Club jsonClub = (from club in jsonClubs where club.Id == newClub.ClubId select club).First();

                // get this clubs league
                LeagueRecord leagueRecord = (from league in db.Leagues where league.LeagueId == jsonClub.LeagueId select league).FirstOrDefault();

                // link up
                if (leagueRecord != null) newClub.League = leagueRecord;

                count = ReportProgress(worker, count, total);
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

                count = ReportProgress(worker, count, total);
            }

            // submit the country changes
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

                count = ReportProgress(worker, count, total);
            }

            // submit the player changes
            db.SubmitChanges();


            // now for each newly created position above, link it up
            foreach (PositionRecord newPosition in positionsEntities)
            {
                // get this positions json
                Position jsonPosition = (from position in jsonPositions where position.Id == newPosition.PositionId select position).First();

                // get this positions zone
                ZoneRecord zoneRecord = (from zone in db.Zones where zone.ZoneId == jsonPosition.ZoneId select zone).FirstOrDefault();

                // link up
                if (zoneRecord != null) newPosition.Zone = zoneRecord;

                count = ReportProgress(worker, count, total);
            }

            // submit the position changes
            db.SubmitChanges();
        }
*/
    }
}