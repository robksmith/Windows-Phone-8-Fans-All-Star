using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Resources;
using Zengo.WP8.FAS.Helpers;
using Zengo.WP8.FAS.Models;
using Newtonsoft.Json;

namespace Zengo.WP8.FAS.ViewModels
{
    public class FreeEntryViewModel : INotifyPropertyChanged
    {
        private const string AnswerLocation = "Resources/QuestionResponses";

        private ObservableCollection<LeagueAnswer> leagueAnswers;
        public ObservableCollection<LeagueAnswer> LeagueAnswers
        {
            get { return leagueAnswers; } 
            set
            {
                if (leagueAnswers != value)
                {
                    leagueAnswers = value;
                    OnPropertyChanged("LeagueAnswers");
                }
            }
        }

        private ObservableCollection<SportAnswer> sportAnswers;
        public ObservableCollection<SportAnswer> SportAnswers
        {
            get { return sportAnswers; }
            set
            {
                if (sportAnswers != value)
                {
                    sportAnswers = value;
                    OnPropertyChanged("SportAnswers");
                }
            }
        }

        private ObservableCollection<StadiumAnswer> stadiumAnswers;
        public ObservableCollection<StadiumAnswer> StadiumAnswers
        {
            get { return stadiumAnswers; }
            set
            {
                if (stadiumAnswers != value)
                {
                    stadiumAnswers = value;
                    OnPropertyChanged("StadiumAnswers");
                }
            }
        }

        public FreeEntryViewModel()
        {
            LeagueAnswers = new ObservableCollection<LeagueAnswer>();
            SportAnswers = new ObservableCollection<SportAnswer>();
            StadiumAnswers = new ObservableCollection<StadiumAnswer>();
        }

        public void ReadBatch()
        {
            string full = string.Format("/Zengo.WP8.FAS;component/{0}/", AnswerLocation);

            StreamResourceInfo leagueSri = Application.GetResourceStream(new Uri(full + string.Format("leagues-{0}.json", App.LanguageViewModel.CurrentLanguage.Locale), UriKind.Relative));
            var leagues = JsonConvert.DeserializeObject<LeagueAnswerResponses>(new StreamReader(leagueSri.Stream).ReadToEnd());
            leagues.Leagues.ToList().ForEach(LeagueAnswers.Add);

            StreamResourceInfo sportSri = Application.GetResourceStream(new Uri(full + string.Format("sports-{0}.json", App.LanguageViewModel.CurrentLanguage.Locale), UriKind.Relative));
            var sports = JsonConvert.DeserializeObject<SportAnswerResponses>(new StreamReader(sportSri.Stream).ReadToEnd());
            sports.Sports.ToList().ForEach(SportAnswers.Add);

            StreamResourceInfo stadiumSri = Application.GetResourceStream(new Uri(full + string.Format("stadium-{0}.json", App.LanguageViewModel.CurrentLanguage.Locale), UriKind.Relative));
            var stadium = JsonConvert.DeserializeObject<StadiumAnswerResponses>(new StreamReader(stadiumSri.Stream).ReadToEnd());
            stadium.Stadiums.ToList().ForEach(StadiumAnswers.Add);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public IEnumerable Leagues(ListSortDirection sortDirection, string searchTerm)
        {
            switch (sortDirection)
            {
                case ListSortDirection.Ascending:
                    {
                        if (string.IsNullOrEmpty(searchTerm))
                        {
                            return from league in LeagueAnswers
                                   orderby league.LeagueName ascending
                                   group league by LeagueNameChooseSortLetter(league)
                                   into c
                                   orderby c.Key ascending
                                   select new PublicGrouping<string, LeagueAnswer>(c);

                        }
                        return from league in LeagueAnswers
                               where league.LeagueName.ToLower().Contains(searchTerm.ToLower())
                               orderby league.LeagueName ascending
                               group league by LeagueNameChooseSortLetter(league)
                               into c
                               orderby c.Key ascending
                               select new PublicGrouping<string, LeagueAnswer>(c);
                    }
                case ListSortDirection.Descending:
                    {
                        if (string.IsNullOrEmpty(searchTerm))
                        {
                            return from league in LeagueAnswers
                                   orderby league.LeagueName descending
                                   group league by LeagueNameChooseSortLetter(league)
                                   into c
                                   orderby c.Key descending
                                   select new PublicGrouping<string, LeagueAnswer>(c);

                        }
                        return from league in LeagueAnswers
                               where league.LeagueName.ToLower().Contains(searchTerm.ToLower())
                               orderby league.LeagueName descending
                               group league by LeagueNameChooseSortLetter(league)
                               into c
                               orderby c.Key descending
                               select new PublicGrouping<string, LeagueAnswer>(c);
                    }
            }
            return null;
        }

        public static string LeagueNameChooseSortLetter(LeagueAnswer league)
        {
            return league.LeagueName.Remove(1).ToUpper();
        }

        public IEnumerable Stadiums(ListSortDirection sortDirection, string searchTerm)
        {
            switch (sortDirection)
            {
                case ListSortDirection.Ascending:
                    {
                        if (string.IsNullOrEmpty(searchTerm))
                        {
                            return from stadium in StadiumAnswers
                                   orderby stadium.StadiumName ascending
                                   group stadium by StadiumNameChooseSortLetter(stadium)
                                       into c
                                       orderby c.Key ascending
                                       select new PublicGrouping<string, StadiumAnswer>(c);

                        }
                        return from stadium in StadiumAnswers
                               where stadium.StadiumName.ToLower().Contains(searchTerm.ToLower())
                               orderby stadium.StadiumName ascending
                               group stadium by StadiumNameChooseSortLetter(stadium)
                                   into c
                                   orderby c.Key ascending
                                   select new PublicGrouping<string, StadiumAnswer>(c);
                    }
                case ListSortDirection.Descending:
                    {
                        if (string.IsNullOrEmpty(searchTerm))
                        {
                            return from stadium in StadiumAnswers
                                   orderby stadium.StadiumName descending
                                   group stadium by StadiumNameChooseSortLetter(stadium)
                                       into c
                                       orderby c.Key descending
                                       select new PublicGrouping<string, StadiumAnswer>(c);

                        }
                        return from stadium in StadiumAnswers
                               where stadium.StadiumName.ToLower().Contains(searchTerm.ToLower())
                               orderby stadium.StadiumName descending
                               group stadium by StadiumNameChooseSortLetter(stadium)
                                   into c
                                   orderby c.Key descending
                                   select new PublicGrouping<string, StadiumAnswer>(c);
                    }
            }
            return null;
        }

        public static string StadiumNameChooseSortLetter(StadiumAnswer league)
        {
            return league.StadiumName.Remove(1).ToUpper();
        }

        public IEnumerable Sports(ListSortDirection sortDirection, string searchTerm)
        {
            switch (sortDirection)
            {
                case ListSortDirection.Ascending:
                    {
                        if (string.IsNullOrEmpty(searchTerm))
                        {
                            return from sport in SportAnswers
                                   orderby sport.SportName ascending
                                   group sport by SportNameChooseSortLetter(sport)
                                       into c
                                       orderby c.Key ascending
                                       select new PublicGrouping<string, SportAnswer>(c);

                        }
                        return from sport in sportAnswers
                               where sport.SportName.ToLower().Contains(searchTerm.ToLower())
                               orderby sport.SportName ascending
                               group sport by SportNameChooseSortLetter(sport)
                                   into c
                                   orderby c.Key ascending
                                   select new PublicGrouping<string, SportAnswer>(c);
                    }
                case ListSortDirection.Descending:
                    {
                        if (string.IsNullOrEmpty(searchTerm))
                        {
                            return from sport in SportAnswers
                                   orderby sport.SportName descending
                                   group sport by SportNameChooseSortLetter(sport)
                                       into c
                                       orderby c.Key descending
                                       select new PublicGrouping<string, SportAnswer>(c);

                        }
                        return from sport in SportAnswers
                               where sport.SportName.ToLower().Contains(searchTerm.ToLower())
                               orderby sport.SportName descending
                               group sport by SportNameChooseSortLetter(sport)
                                   into c
                                   orderby c.Key descending
                                   select new PublicGrouping<string, SportAnswer>(c);
                    }
            }
            return null;
        }

        public static string SportNameChooseSortLetter(SportAnswer league)
        {
            return league.SportName.Remove(1).ToUpper();
        }
    }
}
