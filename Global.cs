using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VaderConsulting.Dependency
{
    public static class Global
    {
        #region Enums

        public enum HealthState : int
        {
            Calculating = -2,
            Unknown = -1,
            OK = 0,
            Degraded = 1,
            Error = 2
        }

        //public enum UserHealthState : int
        //{
        //    Unknown = -1,
        //    OK = 0,
        //    Degraded = 1,
        //    Error = 2

        //}

        public enum RecoveryTier : int
        {
            Zero = 0,
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4,
            Undefined = 999
        }

        public enum RecoveryStream : int
        {
            /// <summary>
            /// Recovery Stream has not been evaluated
            /// </summary>
            Undefined = -5, // not evaluated
            /// <summary>
            /// No Recovery is desired (i.e. H.A.)
            /// </summary>
            None3 = -3,     // No Recovery desired (i.e. HA)
            /// <summary>
            /// No Recovery is possible
            /// </summary>
            None2 = -2,     // No recovery possible (i.e. wrong Site)
            /// <summary>
            /// Recovery is not controlled (usually controlled externally)
            /// </summary>
            None1 = -1,     // Not controlled (external party controls Service)
            /// <summary>
            /// Highly Available
            /// </summary>
            A = 0,          // Highly Available
            /// <summary>
            /// Recovery by SRM
            /// </summary>
            B = 1,          // SRM
            /// <summary>
            /// Restore from Backup
            /// </summary>
            C = 2,          // Restore from backup
            /// <summary>
            /// Reserved for future use
            /// </summary>
            D = 3,          // Reserved for future use
            /// <summary>
            /// External
            /// </summary>
            E = 4,          // External
            /// <summary>
            /// Other Recovery method
            /// </summary>
            Other = 5
        }

        public enum DrawingStatus : int
        {
            Not_Applicable = -1,
            Draft = 0,
            Pending_Review = 1,
            Approved = 2
        }

        public enum AvailabilityPredictionMethod : int
        {
            Time = 0,
            Storage = 1,
            TimeAndStorage = 2
        }

        #endregion

        public static BusinessApplication GetExistingBusinessApplication(string BusinessApplicationName, List<BusinessApplication> BusinessApplications)
        {
            return GetExistingBusinessApplication(new BusinessApplication(BusinessApplicationName), BusinessApplications);
        }

        public static BusinessApplication GetExistingBusinessApplication(BusinessApplication BusinessApplication, List<BusinessApplication> BusinessApplications)
        {
            if (BusinessApplications == null || BusinessApplication == null)
            {
                return null;
            }
            else
            {
                return BusinessApplications.Find(x => x.Name == BusinessApplication.Name);
            }
        }

        public static Server GetExistingServer(string ServerName, List<Server> Servers)
        {
            return GetExistingServer(new Server(ServerName), Servers);
        }

        public static Server GetExistingServer(Server Server, List<Server> Servers)
        {
            if (Servers == null)
            {
                return null;
            }

            if (Server == null)
            {
                return null;
            }
            else
            {
                return Servers.Find(x => Server.Name.StartsWith(x.Name));
            }
        }

        public static HARelationship GetExistingHARelationship(HARelationship Relationship, List<HARelationship> Relationships)
        {
            if (Relationship == null)
            {
                return null;
            }
            else
            {
                return Relationships.Find(x =>
                                              (x.Server1 == Relationship.Server1 && x.Server2 == Relationship.Server2 && x.Service.Name == Relationship.Service.Name)
                                           || (x.Server1 == Relationship.Server2 && x.Server2 == Relationship.Server1 && x.Service.Name == Relationship.Service.Name)
                                         );
            }
        }

        public static Relationship GetExistingRelationship(Relationship Relationship, List<Relationship> Relationships)
        {
            if (Relationship == null)
            {
                return null;
            }
            else
            {
                return Relationships.Find(x => x.Name == Relationship.Name);
            }
        }

        public static Relationship GetRelationship(BusinessApplication FromBusinessApplication, BusinessApplication ToBusinessApplication, List<Relationship> Relationships)
        {
            return Relationships.Find(x => FromBusinessApplication == x.FromBusinessApplication && x.ToBusinessApplication == ToBusinessApplication);
        }

        public static Relationship GetRelationship(BusinessApplication FromBusinessApplication, Server ToServer, List<Relationship> Relationships)
        {
            return Relationships.Find(x => FromBusinessApplication == x.FromBusinessApplication && ToServer == x.ToServer);
        }

        public static int GetBusinessApplicationStateImageIndex(BusinessApplication BusinessApplication, bool ShowDrawingStatusOnBusinessApplication)
        {
            Int32 Result = 0;

            switch (BusinessApplication.SystemState)
            {
                case HealthState.Calculating:
                    Result = 0; // Blue
                    break;
                case HealthState.Unknown:
                    Result = 1; // Grey
                    break;
                case HealthState.Error:
                    Result = 2; // Red
                    break;
                case HealthState.Degraded:
                    Result = 3; // Orange
                    break;
                case HealthState.OK:
                    Result = 4; // Green
                    break;
            }

            if (BusinessApplication.InScope == false)
            {
                Result = 1; // Grey
            }

            return Result;
        }

        public static int GetDrawingStatusImageIndex(Drawing Drawing)
        {
            Int32 Result = 0;

            if (Drawing == null)
            {
                return 11; // No drawing
            }

            switch (Drawing.Status)
            {
                case DrawingStatus.Not_Applicable: // No status
                    Result = 12;
                    break;
                case DrawingStatus.Draft:
                    Result = 13;
                    break;
                case DrawingStatus.Pending_Review:
                    Result = 14;
                    break;
                case DrawingStatus.Approved:
                    Result = 15;
                    break;
            }

            return Result;
        }

        public static int GetServerStateImageIndex(Server Server)
        {
            Int32 Result = -1;

            switch (Server.SystemState)
            {
                case Global.HealthState.Unknown:
                    Result = 6;
                    break;
                case Global.HealthState.Error:
                    Result = 7;
                    break;
                case Global.HealthState.Degraded:
                    Result = 8;
                    break;
                case Global.HealthState.OK:
                    Result = 9;
                    break;
            }

            return Result;
        }

        public static int GetDrawingImageIndex(Drawing Drawing)
        {
            Int32 Result = -1;

            if (Drawing != null)
            {
                if (VaderConsulting.Helper.Properties.FlagDrawingErrors)
                {
                    if (Drawing.InError)
                    {
                        Result = 13;
                    }
                    else
                    {
                        switch (Drawing.Status)
                        {
                            case DrawingStatus.Not_Applicable:
                                Result = 11;
                                break;
                            case DrawingStatus.Draft:
                                Result = 12;
                                break;
                            case DrawingStatus.Pending_Review:
                                Result = 14;
                                break;
                            case DrawingStatus.Approved:
                                Result = 15;
                                break;
                        }
                    }
                }
                else
                {
                    switch (Drawing.Status)
                    {
                        case DrawingStatus.Not_Applicable:
                            Result = 12;
                            break;
                        case DrawingStatus.Draft:
                            Result = 13;
                            break;
                        case DrawingStatus.Pending_Review:
                            Result = 14;
                            break;
                        case DrawingStatus.Approved:
                            Result = 15;
                            break;
                    }
                }
            }
            else
            {
                Result = 11;
            }

            return Result;
        }

        public static Bitmap GetServerImage(Server Server, ImageList Images)
        {
            Bitmap Result = null;
            int BackgroundImageIndex = 0;

            switch (Server.SystemState)
            {
                case HealthState.Unknown:
                    BackgroundImageIndex = 6;
                    break;
                case Global.HealthState.Error:
                    BackgroundImageIndex = 7;
                    break;
                case Global.HealthState.Degraded:
                    BackgroundImageIndex = 8;
                    break;
                case Global.HealthState.OK:
                    BackgroundImageIndex = 9;
                    break;
            }

            Result = new Bitmap(Images.ImageSize.Width, Images.ImageSize.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(Result);

            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

            g.DrawImage(Images.Images[BackgroundImageIndex], 0, 0);

            //if (!ShowInRunbook)
            //{
            //    g.DrawImage(Images.Images[6], 0, 0);
            //}

            return Result;
        }

        public static string GetTierName(RecoveryTier Tier)
        {
            string Result = "";

            switch (Tier)
            {
                case RecoveryTier.Zero:
                    Result = "0";
                    break;
                case RecoveryTier.One:
                    Result = "1";
                    break;
                case RecoveryTier.Two:
                    Result = "2";
                    break;
                case RecoveryTier.Three:
                    Result = "3";
                    break;
                case RecoveryTier.Four:
                    Result = "4";
                    break;
                default:
                case RecoveryTier.Undefined:
                    Result = "";
                    break;
            }

            return Result;
        }

        public static string GetBusinessApplicationStreamName(RecoveryStream Stream)
        {
            string Result = "";

            switch (Stream)
            {
                case RecoveryStream.Undefined:
                    Result = "?";
                    break;
                case RecoveryStream.None3:  // No Recovery desired (i.e. HA)
                    Result = "A";
                    break;
                case RecoveryStream.None2:  // No recovery possible (i.e. wrong Site)
                    Result = "None";
                    break;
                case RecoveryStream.None1:  // Not controlled (external party controls Service)
                    Result = "Ext.";
                    break;
                case RecoveryStream.A:      // **** should never happen *****
                    Result = " ";
                    break;
                case RecoveryStream.B:      // SRM
                    Result = "B";
                    break;
                case RecoveryStream.C:      // Restore from backup
                    Result = "C";
                    break;
                case RecoveryStream.D:      // Reserved for future use
                    Result = "D";
                    break;
                case RecoveryStream.E:      // External
                    Result = "E";
                    break;
                default:
                case RecoveryStream.Other:  // Something we haven't thought of
                    Result = "O";
                    break;

            }

            return Result;
        }

        public static string GetServerStreamName(Server Server)
        {
            string Result = "";

            //if (Server.HARelationships.Count > 0)
            //{
            //    Result = "-";

            //    // Fix this now
            //    Server.Stream = RecoveryStream.None3;
            //}
            //else
            //{
            switch (Server.Stream)
            {
                case RecoveryStream.Undefined:
                    Result = "?";
                    break;
                case RecoveryStream.None3:  // No Recovery desired (i.e. HA)
                    Result = "None";
                    break;
                case RecoveryStream.None2:  // No recovery possible (i.e. wrong Site)
                    Result = "None";
                    break;
                case RecoveryStream.None1:  // Not controlled (external party controls Service)
                    Result = "None";
                    break;
                case RecoveryStream.A:      // **** should never happen *****
                    Result = " ";
                    break;
                case RecoveryStream.B:      // SRM
                    Result = "B";
                    break;
                case RecoveryStream.C:      // Restore from backup
                    Result = "C";
                    break;
                case RecoveryStream.D:      // Reserved for future use
                    Result = "D";
                    break;
                case RecoveryStream.E:      // External
                    Result = "E";
                    break;
                default:
                case RecoveryStream.Other:  // Something we haven't thought of
                    Result = "O";
                    break;

            }
            //}

            return Result;
        }

        //public static RecoveryStream CalculateServiceStream(BusinessApplication Service)
        //{
        //    RecoveryStream Result = RecoveryStream.Undefined;



        //    return Result;
        //}

        //public static RecoveryStream CalculateRecoveryStream(List<Server> Servers)
        //{
        //    RecoveryStream Result = RecoveryStream.Undefined;

        //    foreach (Server Server in Servers)
        //    {
        //        // Not HA, so process normally
        //        if (Server.Stream > Result)
        //        {
        //            Result = Server.Stream;
        //        }
        //    }

        //    return Result;
        //}

        public static RecoveryStream CalculateHARecoveryStream(List<Server> Servers)
        {
            //RecoveryStream ProtectedSiteResult = RecoveryStream.Undefined;
            //RecoveryStream RecoverySiteResult = RecoveryStream.Undefined;
            RecoveryStream ExternalSiteResult = RecoveryStream.Undefined;
            //RecoveryStream NotProtectedSiteResult = RecoveryStream.Undefined;
            RecoveryStream OverallResult = RecoveryStream.Undefined;

            foreach (Server Server in Servers)
            {
                if (Server.PhysicalSite != null)
                {
                    switch (Server.PhysicalSite.SiteType)
                    {
                        case PhysicalSite.PhysicalSiteType.Protected:
                            //ProtectedSiteResult = RecoveryStream.None3;
                            break;
                        case PhysicalSite.PhysicalSiteType.Recovery:
                            //RecoverySiteResult = RecoveryStream.None2;
                            break;
                        case PhysicalSite.PhysicalSiteType.NotProtected:
                            if (Server.PhysicalSite.Internal)
                            {
                                //NotProtectedSiteResult = RecoveryStream.None1;
                            }
                            else
                            {
                                if (Server.Stream > ExternalSiteResult)
                                {
                                    ExternalSiteResult = Server.Stream;
                                }
                            }

                            break;
                    }
                }
                else
                {
                    // Physical Site not defined.
                }

                if (Server.Stream > OverallResult)
                {
                    OverallResult = Server.Stream;
                }
            }

            return OverallResult;
        }

        public static PhysicalSite GetExistingPhysicalSite(string SiteName, List<PhysicalSite> Sites)
        {
            if (Sites == null || Sites.Count == 0)
            {
                return null;
            }
            else
            {
                return Sites.Find(x => x.Name == SiteName);
            }
        }
    }
}
