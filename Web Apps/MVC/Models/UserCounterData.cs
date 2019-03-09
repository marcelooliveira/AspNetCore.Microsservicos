using System.Collections.Generic;

namespace MVC.Models
{
    public class UserCounterData
    {
        public UserCounterData()
        {
        }

        public UserCounterData(List<UserNotification> userNotifications, int userBasketCount)
        {
            if (userNotifications != null)
            {
                Notifications = userNotifications;
            }
            BasketCount = userBasketCount;
        }

        public List<UserNotification> 
            Notifications { get; set; } = new List<UserNotification>();
        public int BasketCount { get; set; }
    }
}
