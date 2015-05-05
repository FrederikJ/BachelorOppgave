using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HovedOppgave.Classes
{
    /// <summary>
    /// Setter opp parametre som går over hele prosjektet
    /// 
    /// Gjeste forfatter: 
    /// </summary>
    
    public class Constant
    {
        public enum Rights { Administrator, User, Guest };
        public enum NotificationType { success, info, warning, danger };
        public const int SaltSize = 40;
    }
}