using Microsoft.AspNetCore.Mvc.ViewFeatures;
using static StoreFront.Domain.Enums.NotificationEnums;

namespace StoreFront.Application.Helpers
{
    public static class NotificationHelper
    {
        private const string NotifTypeKey = "NotifType";
        private const string NotifMessageKey = "NotifMessage";
        private const string NotifTitleKey = "NotifTitle";

        public static void CreateNotification(ITempDataDictionary tempData, NotificationType type, string message, string title)
        {
            tempData[NotifTypeKey] = type.ToString();
            tempData[NotifMessageKey] = message;
            tempData[NotifTitleKey] = title;
        }

        public static void SetSuccess(this ITempDataDictionary tempData, string message, string title)
        => CreateNotification(tempData, NotificationType.success, message, title);

        public static void SetError(this ITempDataDictionary tempData, string message, string title)
            => CreateNotification(tempData, NotificationType.error, message, title);

        public static void SetWarning(this ITempDataDictionary tempData, string message, string title)
            => CreateNotification(tempData, NotificationType.warning, message, title);

        public static void SetInfo(this ITempDataDictionary tempData, string message, string title)
            => CreateNotification(tempData, NotificationType.info, message, title);
    } 
}
