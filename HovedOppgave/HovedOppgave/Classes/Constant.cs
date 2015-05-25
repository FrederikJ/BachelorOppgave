using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Classes
{
    /// <summary>
    /// Setter opp parametre som går over hele prosjektet
    /// 
    /// Gjeste forfatter: Fant ikke, men kommer fra prosjekt som Frederik har vært med i
    /// selv om konstant navnene har blitt endret til å passe inn i vårt system
    /// </summary>
    
    public class Constant
    {
        public enum Rights { Administrator, User, Guest };
        public enum NotificationType { success, info, warning, danger };
        public const int SaltSize = 40;
    }
}