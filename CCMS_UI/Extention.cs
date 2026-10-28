using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CCMSUI
{
    public class Extension
    {
        // Methods
        public static string ChildDeviceStatusToStr(string str)
        {
            switch (str)
            {
                case "0":
                    return "Unknown";

                case "1":
                    return "Healthy";

                case "2":
                    return "Attention Soon";

                case "3":
                    return "Attention Now";

                case "4":
                    return "Not Configured";
            }
            return "Unknown";
        }

        public static string getAggregateStateImageUrl(string AggregateState)
        {
            switch (AggregateState)
            {
                case "Healthy":
                    return "~/images/healthy.gif";

                case "Attention Soon":
                    return "~/images/Attention Soon.gif";

                case "Attention Now":
                    return "~/images/Attention Now.gif";

                case "Not Configured":
                    return "~/images/not-configured.jpg";
            }
            return "~/images/unknown.jpg";
        }

        public static string getCalculatedStateImageUrl(string CalculatedState)
        {
            switch (CalculatedState)
            {
                case "Healthy":
                    return "~/images/healthy.gif";

                case "Attention Soon":
                    return "~/images/Attention Soon.gif";

                case "Attention Now":
                    return "~/images/Attention Now.gif";

                case "Not Configured":
                    return "~/images/not-configured.jpg";

                case "Unknown":
                    return "~/images/unknown.jpg";
            }
            return "~/images/unknown.jpg";
        }

        public static string getChildDeviceStatusImages(string str)
        {
            switch (str)
            {
                case "0":
                    return "../images/Unknown.jpg";

                case "1":
                    return "../images/Healthy.gif";

                case "2":
                    return "../images/attention-soon.gif";

                case "3":
                    return "../images/attention-now.gif";

                case "4":
                    return "../images/not-configured.jpg";
            }
            return "../images/Unknown.jpg";
        }

        public static string getCurrentModeImageUrl(string CurrentMode)
        {
            switch (CurrentMode)
            {
                case "In Service":
                    return "~/images/Inservice1.jpg";

                case "Supervisor":
                    return "~/images/Supervisor.jpg";

                case "Initialize":
                    return "~/images/initilize.jpg";

                case "Powerup":
                    return "~/images/Powerup1.jpg";

                case "Unknown":
                    return "~/images/unknown.jpg";

                case "Not Configured":
                    return "~/images/not-configured.jpg";

                case "Offline":
                    return "~/images/Offline1.jpg";

                case "Out of Service":
                    return "~/images/out-of-service.jpg";

                case "Unreachable":
                    return "~/images/Unreachable.jpg";

                case "Suspend":
                    return "~/images/Suspend.png";

                case "Agent Error":
                    return "~/images/agent-error.jpg";
            }
            return "~/images/unknown.jpg";
        }

        public static string getParentDeviceStatusImages(string str)
        {
            switch (str)
            {
                case "0":
                    return "../images/Good.gif";

                case "1":
                    return "../images/Routine.png";

                case "2":
                    return "../images/Warning.gif";

                case "3":
                    return "../images/Suspend.png";

                case "4":
                    return "../images/Fatal.gif";
            }
            return "../images/Unknown.jpg";
        }

        public static string GetSupplyStateDescription(string iValue)
        {
            string str = "";
            string str2 = iValue;
            if (str2 == null)
            {
                return str;
            }
            if (!(str2 == "0"))
            {
                if (str2 != "1")
                {
                    if (str2 == "2")
                    {
                        return "Attention Soon";
                    }
                    if (str2 == "3")
                    {
                        return "Attention Now";
                    }
                    if (str2 != "4")
                    {
                        return str;
                    }
                    return "Not Configured";
                }
            }
            else
            {
                return "Unknown";
            }
            return "Healthy";
        }

        public static string getSupplyStateImageUrl(string SupplyState)
        {
            switch (SupplyState)
            {
                case "Healthy":
                    return "~/images/healthy.gif";

                case "Attention Soon":
                    return "~/images/Attention Soon.gif";

                case "Attention Now":
                    return "~/images/Attention Now.gif";

                case "Not Configured":
                    return "~/images/not-configured.jpg";

                case "Unknown":
                    return "~/images/unknown.jpg";
            }
            return "~/images/unknown.jpg";
        }

        public static string ParentDeviceStatusToStr(string str)
        {
            switch (str)
            {
                case "0":
                    return "Good";

                case "1":
                    return "Routine";

                case "2":
                    return "Warning";

                case "3":
                    return "Suspend";

                case "4":
                    return "Fatal";
            }
            return "Unknown";
        }
    }





}