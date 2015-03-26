using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Classes
{
    public class Reports
    {

    }
    #region eksempel tatt fra systemutvikling prosjektet
    /*
     * Gjeste forfatter: Eivind Skreddernes & Andreas Dyrøy
     * 
    public static int TEAMRAPPORT = 0;
        public static int PROSJEKTRAPPORT = 1;
        public static int INDIVIDRAPPORT = 2;

        private Team team;
        private Prosjekt prosjekt;
        private Bruker bruker;
        private string info;
        private string prosjektRapportForBruker;
        private TimeSpan bruktTidProsjekt;
        private TimeSpan estimertTidProsjekt;
        private TimeSpan sumTimerForBruker;
        private TimeSpan sumFullforteTimerForBruker;
        private int antallFerdigeOppgaver;
        private int antallOppgaver;
        private int antallDeltakerePaTeam;
        private int antallFaser;

        /// <summary>
        /// Lag en rapport
        /// </summary>
        /// <param name="type">Type rapport</param>
        /// <param name="_id">Id til det som skal med</param>
        public Rapport(int type, int _id)
        {
            info = "";
            bruktTidProsjekt = new TimeSpan();
            estimertTidProsjekt = new TimeSpan();
            sumTimerForBruker = new TimeSpan();
            antallFerdigeOppgaver = 0;
            antallDeltakerePaTeam = 0;

            switch (type) {
                case 0:
                    {
                        team = Queries.GetTeamById(_id);
                        if (team != null)
                        {
                            info += "Navn på team: " + team.Navn;
                            info += "<br />Opprettet: " + team.Opprettet.ToShortDateString();
                            // Teamleder?
                            info += "<br /><h3>Deltakere:</h3>";
                            foreach (Bruker b in team.Brukere)
                                info += "<br /><t />" + b.ToString();
                            
                            info += "<br /><h3>Prosjekter:</h3>";
                            info += "<p>";

                            foreach (Prosjekt p in team.Prosjekter)
                            {
                            
                                info += "<br /><t />" + p.Navn;
                                Bruker faseleder = SessionSjekk.GetFaseleder(p.Prosjekt_id);
                                if(faseleder != null)   
                                    info += "<br />Faseleder: " + SessionSjekk.GetFaseleder(p.Prosjekt_id).ToString();
                            }
                            info += "</p>";

                            info += "<br />";
                                antallDeltakerePaTeam = team.Brukere.Count;
                            
                        }
                    }
                    break;
                case 1:
                    {
                        prosjekt = Queries.GetProsjekt(_id);
                        if (prosjekt != null)
                        {
                            List<Fase> faseListe = Queries.GetFaseForProsjekt(_id);
                            info += "Navn på prosjekt: " + prosjekt.Navn;
                            info += "<br />Opprettet: " + prosjekt.Opprettet.ToShortDateString();
                            DateTime start = (DateTime)prosjekt.StartDato;
                            DateTime slutt = (DateTime)prosjekt.SluttDato;
                            info += "<br />Startdato: " + start.ToShortDateString();
                            info += "<br />Sluttdato: " + slutt.ToShortDateString();
                            Bruker prosjektLeder = Queries.GetBruker(prosjekt.Bruker_id);
                            info += "<br />Prosjektleder: " + prosjektLeder.ToString();

                            info += "<br />Team: " + prosjekt.Team.Navn;

                            info += "<br /><h2>Faser:</h2>";

                            antallFaser = faseListe.Count;

                            foreach (Fase f in faseListe)
                            {
                                info += "<h3>Navn: " + f.Navn + "</h3>";
                                info += "Faseleder: " + f.Bruker.ToString();
                                info += " | Opprettet: " + f.Opprettet.ToShortDateString();
                                info += " | Startdato: " + f.Start.ToShortDateString();
                                info += " | Sluttdato: " + f.Stopp.ToShortDateString();
                                info += "<div class=\"rapport2\">";
                                foreach (Oppgave o in f.Oppgaver)
                                {
                                    info += "<br /><h4>Navn: " + o.Tittel + "</h4>";
                                    info += "ID: " + o.RefOppgaveId;
                                    info += " | Opprettet: " + o.Opprettet.ToShortDateString();
                                    info += " | User story: " + o.UserStory;
                                    info += " | Krav: " + o.Krav;
                                    int avhengigOppgave = Validator.SjekkAvhengighet(o.Oppgave_id);
                                    info += " | Avhengighet: " + (avhengigOppgave == -1 ? "Nei" : "Ja");
                                    info += "<br />Estimat: " + o.Estimat;

                                    int test1 = o.Oppgave_id;
                                    List<Time> timeListe = Queries.GetTimerForOppgave(o.Oppgave_id);
                                    TimeSpan bruktTid = new TimeSpan(0);
                                    foreach (Time t in timeListe)
                                    {
                                        bruktTidProsjekt += t.Tid;
                                        bruktTid += t.Tid;

                                    }
                                        
                                    info += " | Brukt tid: " + bruktTid.ToString();
                                    Status status = Queries.GetStatus(o.Status_id);
                                    info += " | Status: " + status.Navn;
                                    estimertTidProsjekt += (TimeSpan)o.Estimat;
                                    
                                }
                                info += "</div>";
                                info += "<hr />";

                            }
                            
                        }

                    }
                    break;
                case 2:
                    {
                        bruker = Queries.GetBruker(_id);
                        if (bruker != null)
                        {
                            info += "Navn på bruker: " + bruker.ToString();
                            info += "<br />Internt brukernavn: " + bruker.Brukernavn;
                            info += "<br />Lagt til: " + bruker.Opprettet.ToShortDateString();
                            info += "<br />E-post: " + bruker.Epost;
                            
                            info += "<br /><h3>Prosjekter:</h3>";
                            info += "<div class=\"rapport2\">";
                            List<Prosjekt> prosjektListe = Queries.GetAlleAktiveProsjekterForBruker(_id);
                            foreach (Prosjekt p in prosjektListe)
                            {
                                info += p.Navn + "<br />";
                                prosjektRapportForBruker += "<br />Navn: " + p.Navn;
                                // vi tar det sia
                                if (SessionSjekk.IsFaseleder(_id, p.Prosjekt_id))
                                    prosjektRapportForBruker += " | " + "Min rolle: faseleder. ";
                                else
                                    prosjektRapportForBruker += " | " + "Min rolle: " + (_id == p.Bruker_id ? "Prosjektleder" : "Utvikler");
                            }
                            info += "</div>";
                            
                            info += "<br /><h3>Team:</h3>";
                            info += "<div class=\"rapport2\">";
                            foreach (Team t in bruker.Teams)
                                info += t.Navn + "<br />";
                            info += "</div>";

                            info += "<h3>Oppgaver:</h3>";
                            info += "<div class=\"rapport2\">";
                            List<Time> timeListe = Queries.GetTimerForBruker(_id);
                            if (timeListe != null)
                            {
                                List<Oppgave> oppgaverTilBruker = Queries.GetAlleAktiveOppgaverForBruker(_id);
                                antallOppgaver = oppgaverTilBruker.Count;

                                foreach (Oppgave oppgave in oppgaverTilBruker)
                                {
                                    TimeSpan sumTimerForBrukerOppgave = new TimeSpan(0);
                                    foreach (Time time in timeListe)
                                    {
                                        if (time.Oppgave_id == oppgave.Oppgave_id)
                                        {
                                            sumTimerForBrukerOppgave += time.Tid;
                                            sumTimerForBruker += time.Tid;
                                        }
                                    }
                                    info += "<h4>" + oppgave.Tittel + " Brukt tid: " + sumTimerForBrukerOppgave.ToString() + "</h4>";
                                    prosjektRapportForBruker += "<br/>" + oppgave.Tittel + " | Brukt tid: " + sumTimerForBrukerOppgave.ToString();
                                    info += "Prosjekt: " + oppgave.Prosjekt.Navn;
                                    info += " | Fase: " + oppgave.Fase.Navn;

                                }
                            }
                            info += "</div>";

                            List<Oppgave> ferdigeOppgaver = Queries.GetAlleFerdigeOppgaverForBruker(_id);
                            antallFerdigeOppgaver = ferdigeOppgaver.Count;
                            foreach (Oppgave o in ferdigeOppgaver)
                                sumFullforteTimerForBruker += (TimeSpan) o.BruktTid;

                            info += "<h3>Hendelser:</h3>";
                            info += "<p>";
                            List<Logg> loggListe = Queries.GetLoggForBruker(_id);
                            foreach (Logg l in loggListe)
                                info += l.Hendelse + "<br />";
                            info += "</p>";
                        }
                    }
                    break;
                default:
                    {
                        info += "Feil valg, bruk Rapport.<noke>";
                    }
                    break;
            }
                
            
        }

        public int getAntallFaser()
        {
            return antallFaser;
        }

        public TimeSpan GetBruktTidPaProsjekt()
        {
            if (bruktTidProsjekt != null)
                return bruktTidProsjekt;
            else
                return new TimeSpan(0);
        }

        public TimeSpan GetEstimatForProsjekt()
        {
            return estimertTidProsjekt;
        }

        public TimeSpan GetProsjektvarighet()
        {
            if (prosjekt != null)
            {
                DateTime start = (DateTime)prosjekt.StartDato;
                DateTime slutt = (DateTime)prosjekt.SluttDato;
                return slutt - start;
            }
            else
            {
                return new TimeSpan(0);
            }
                
        }

        public string VisProsjektRapportForBruker()
        {

            return prosjektRapportForBruker;
        }

        public TimeSpan GetSumTimerForBruker()
        {
            return sumTimerForBruker;
        }

        public TimeSpan GetSumFullforteTimerForBruker()
        {
            return sumFullforteTimerForBruker;
        }

        public int GetAntallFerdigeOppgaverForBruker()
        {
            return antallFerdigeOppgaver;
        }

        public int GetAntallOppgaverForBruker()
        {
            return antallOppgaver;
        }


        public int GetAntallTeammedlemmer()
        {
            return antallDeltakerePaTeam;
        }

        public override string ToString()
        {
            return info;
        }
    */
    #endregion
}