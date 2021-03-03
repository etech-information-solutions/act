using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using ACT.Core.Enums;
using ACT.Core.Models;
using ACT.Core.Models.Custom;
using ACT.Data.Models;

namespace ACT.Core.Services
{
    public class ChepLoadJournalService : BaseService<ChepLoadJournal>, IDisposable
    {
        public ChepLoadJournalService()
        {

        }

        /// <summary>
        /// Gets a total number of Items matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public int Total1( PagingModel pm, CustomSearchModel csm )
        {
            if ( csm.FromDate.HasValue && csm.ToDate.HasValue && csm.FromDate?.Date == csm.ToDate?.Date )
            {
                csm.ToDate = csm.ToDate?.AddDays( 1 );
            }

            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "skip", pm.Skip ) },
                { new SqlParameter( "take", pm.Take ) },
                { new SqlParameter( "cmsClientId", csm.ClientId ) },
                { new SqlParameter( "csmStatus", ( int ) csm.Status ) },
                { new SqlParameter( "csmBalanceStatus", ( int ) csm.BalanceStatus ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmOutstandingReasonId", csm.OutstandingReasonId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmReconciliationStatus", ( int ) csm.ReconciliationStatus ) },
                { new SqlParameter( "cmsDocketNumber", csm.DocketNumber ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmManuallyMatchedUID", csm.ManuallyMatchedUID ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                COUNT(1) AS [Total]
                             FROM
                                [dbo].[ChepLoadJournal] cj
                                INNER JOIN [dbo].[Client] c ON c.Id=cj.ClientId
                                LEFT OUTER JOIN [dbo].[OutstandingReason] r ON r.Id=cj.OutstandingReasonId";

            if ( csm.IsPODOutstanding )
            {
                query = $@"{query} LEFT OUTER JOIN [dbo].[ClientLoad] cl1 ON cl1.[Id]=(SELECT TOP 1 cl2.[Id] FROM [dbo].[ClientLoad] cl2 WHERE cl2.[ReceiverNumber]=cj.[Ref] OR cl2.[ReceiverNumber]=cj.[OtherRef])
                                   LEFT OUTER JOIN [dbo].[Vehicle] v ON v.Id=cl1.[VehicleId]
                                   LEFT OUTER JOIN [dbo].[Transporter] t ON t.Id=cl1.[TransporterId]";
            }

            // WHERE

            #region WHERE

            query = $"{ query} WHERE (1=1)";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=c.[Id] AND pu.[UserId]=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.IsOP )
            {
                query = $"{query} AND (cj.Quantity > 0)";
                query = $"{query} AND (cj.Ref LIKE '5000%')";
            }
            if ( csm.IsPSPPickUp )
            {
                query = $"{query} AND (cj.IsPSPPickup=1)";
            }
            if ( csm.IsPODOutstanding )
            {
                query = $"{query} AND (r.IsPODOutstanding=1)";
            }
            if ( csm.IsTransporterLiable )
            {
                query = $"{query} AND (cj.TransporterLiable=1)";
            }
            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (c.Id=@cmsClientId)";
            }
            if ( csm.BalanceStatus != BalanceStatus.None )
            {
                query = $"{query} AND (cj.BalanceStatus=@csmBalanceStatus)";
            }
            if ( csm.ReconciliationStatus != ReconciliationStatus.All )
            {
                query = $"{query} AND (cj.Status=@csmReconciliationStatus)";
            }
            if ( csm.OutstandingReasonId > 0 )
            {
                query = $"{query} AND (cj.OutstandingReasonId=@csmOutstandingReasonId)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cj.ShipmentDate >= @csmFromDate AND cj.ShipmentDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cj.ShipmentDate >= @csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cj.ShipmentDate <= @csmToDate) ";
                }
            }

            if ( !string.IsNullOrEmpty( csm.ManuallyMatchedUID ) )
            {
                query = $"{query} AND (cj.ManuallyMatchedUID=@csmManuallyMatchedUID) ";
            }

            if ( !string.IsNullOrEmpty( csm.DocketNumber ) )
            {
                query = $"{query} AND (cj.DocketNumber=@csmDocketNumber) ";
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (cj.[ChepStatus] LIKE '%{1}%' OR
                                                  cj.[TransactionType] LIKE '%{1}%' OR
                                                  cj.[DocketNumber] LIKE '%{1}%' OR
                                                  cj.[OriginalDocketNumber] LIKE '%{1}%' OR
                                                  cj.[UMI] LIKE '%{1}%' OR
                                                  cj.[LocationId] LIKE '%{1}%' OR
                                                  cj.[Location] LIKE '%{1}%' OR
                                                  cj.[OtherPartyId] LIKE '%{1}%' OR
                                                  cj.[OtherParty] LIKE '%{1}%' OR
                                                  cj.[OtherPartyCountry] LIKE '%{1}%' OR
                                                  cj.[EquipmentCode] LIKE '%{1}%' OR
                                                  cj.[Equipment] LIKE '%{1}%' OR
                                                  cj.[Ref] LIKE '%{1}%' OR
                                                  cj.[OtherRef] LIKE '%{1}%' OR
                                                  cj.[BatchRef] LIKE '%{1}%' OR
                                                  cj.[InvoiceNumber] LIKE '%{1}%' OR
                                                  cj.[DataSource] LIKE '%{1}%' OR
                                                  cj.[CreatedBy] LIKE '%{1}%' OR
                                                  cj.[Quantity] LIKE '%{1}%' OR
                                                  c.[Email] LIKE '%{1}%' OR
                                                  c.[AdminEmail] LIKE '%{1}%' OR
                                                  c.[AdminPerson] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[ChepReference] LIKE '%{1}%' OR
                                                  c.[CompanyRegistrationNumber] LIKE '%{1}%' OR
                                                  r.[Description] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            CountModel model = context.Database.SqlQuery<CountModel>( query, parameters.ToArray() ).FirstOrDefault();

            return model.Total;
        }

        /// <summary>
        /// Gets a list of Items matching the specified search params
        /// </summary>
        /// <param name="pm"></param>
        /// <param name="csm"></param>
        /// <returns></returns>
        public List<ChepLoadJournalCustomModel> List1( PagingModel pm, CustomSearchModel csm )
        {
            if ( csm.FromDate.HasValue && csm.ToDate.HasValue && csm.FromDate?.Date == csm.ToDate?.Date )
            {
                csm.ToDate = csm.ToDate?.AddDays( 1 );
            }

            // Parameters

            #region Parameters

            List<object> parameters = new List<object>()
            {
                { new SqlParameter( "skip", pm.Skip ) },
                { new SqlParameter( "take", pm.Take ) },
                { new SqlParameter( "cmsClientId", csm.ClientId ) },
                { new SqlParameter( "csmStatus", ( int ) csm.Status ) },
                { new SqlParameter( "query", csm.Query ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmBalanceStatus", ( int ) csm.BalanceStatus ) },
                { new SqlParameter( "csmOutstandingReasonId", csm.OutstandingReasonId ) },
                { new SqlParameter( "csmToDate", csm.ToDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "userid", ( CurrentUser != null ) ? CurrentUser.Id : 0 ) },
                { new SqlParameter( "csmFromDate", csm.FromDate ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmReconciliationStatus", ( int ) csm.ReconciliationStatus ) },
                { new SqlParameter( "cmsDocketNumber", csm.DocketNumber ?? ( object ) DBNull.Value ) },
                { new SqlParameter( "csmManuallyMatchedUID", csm.ManuallyMatchedUID ?? ( object ) DBNull.Value ) },
            };

            #endregion

            string query = @"SELECT
                                cj.*,
                                c.[CompanyName] AS [ClientName],
                                r.[Description] AS [OutstandingReason],
                                (SELECT COUNT(1) FROM [dbo].[ChepLoadJournal] cj WHERE cj.Id=cj.[ChepLoadId]) AS [VersionCount]";

            if ( csm.IsPODOutstanding )
            {
                query = $@"{query} ,t.[Name] AS [TransporterName],
                                    v.[Registration] AS [VehicleRegistration]";
            }

            query = $@"{query} FROM
                                [dbo].[ChepLoadJournal] cj
                                INNER JOIN [dbo].[Client] c ON c.Id=cj.ClientId
                                LEFT OUTER JOIN [dbo].[OutstandingReason] r ON r.Id=cj.OutstandingReasonId";

            if ( csm.IsPODOutstanding )
            {
                query = $@"{query} LEFT OUTER JOIN [dbo].[ClientLoad] cl1 ON cl1.[Id]=(SELECT TOP 1 cl2.[Id] FROM [dbo].[ClientLoad] cl2 WHERE cl2.[ReceiverNumber]=cj.[Ref] OR cl2.[ReceiverNumber]=cj.[OtherRef])
                                   LEFT OUTER JOIN [dbo].[Vehicle] v ON v.Id=cl1.[VehicleId]
                                   LEFT OUTER JOIN [dbo].[Transporter] t ON t.Id=cl1.[TransporterId]";
            }

            // WHERE

            #region WHERE

            query = $"{query} WHERE (1=1)";

            if ( CurrentUser.RoleType == RoleType.PSP )
            {
                query = $"{query} AND EXISTS(SELECT 1 FROM [dbo].[PSPUser] pu, [dbo].[PSPClient] pc WHERE pu.[PSPId]=pc.[PSPId] AND pc.[ClientId]=c.[Id] AND pu.[UserId]=@userid) ";
            }

            #endregion

            // Custom Search

            #region Custom Search

            if ( csm.IsOP )
            {
                query = $"{query} AND (cj.Quantity > 0)";
                query = $"{query} AND (cj.Ref LIKE '5000%')";
            }
            if ( csm.IsPSPPickUp )
            {
                query = $"{query} AND (cj.IsPSPPickup=1)";
            }
            if ( csm.IsPODOutstanding )
            {
                query = $"{query} AND (r.IsPODOutstanding=1)";
            }
            if ( csm.IsTransporterLiable )
            {
                query = $"{query} AND (cj.TransporterLiable=1)";
            }
            if ( csm.ClientId > 0 )
            {
                query = $"{query} AND (c.Id=@cmsClientId)";
            }
            if ( csm.BalanceStatus != BalanceStatus.None )
            {
                query = $"{query} AND (cj.BalanceStatus=@csmBalanceStatus)";
            }
            if ( csm.ReconciliationStatus != ReconciliationStatus.All )
            {
                query = $"{query} AND (cj.Status=@csmReconciliationStatus)";
            }
            if ( csm.OutstandingReasonId > 0 )
            {
                query = $"{query} AND (cj.OutstandingReasonId=@csmOutstandingReasonId)";
            }

            if ( csm.FromDate.HasValue && csm.ToDate.HasValue )
            {
                query = $"{query} AND (cj.ShipmentDate >= @csmFromDate AND cj.ShipmentDate <= @csmToDate) ";
            }
            else if ( csm.FromDate.HasValue || csm.ToDate.HasValue )
            {
                if ( csm.FromDate.HasValue )
                {
                    query = $"{query} AND (cj.ShipmentDate >= @csmFromDate) ";
                }
                if ( csm.ToDate.HasValue )
                {
                    query = $"{query} AND (cj.ShipmentDate <= @csmToDate) ";
                }
            }

            if ( !string.IsNullOrEmpty( csm.ManuallyMatchedUID ) )
            {
                query = $"{query} AND (cj.ManuallyMatchedUID=@csmManuallyMatchedUID) ";
            }

            if ( !string.IsNullOrEmpty( csm.DocketNumber ) )
            {
                query = $"{query} AND (cj.DocketNumber=@csmDocketNumber) ";
            }

            #endregion

            // Normal Search

            #region Normal Search

            if ( !string.IsNullOrEmpty( csm.Query ) )
            {
                query = string.Format( @"{0} AND (cj.[ChepStatus] LIKE '%{1}%' OR
                                                  cj.[TransactionType] LIKE '%{1}%' OR
                                                  cj.[DocketNumber] LIKE '%{1}%' OR
                                                  cj.[OriginalDocketNumber] LIKE '%{1}%' OR
                                                  cj.[UMI] LIKE '%{1}%' OR
                                                  cj.[LocationId] LIKE '%{1}%' OR
                                                  cj.[Location] LIKE '%{1}%' OR
                                                  cj.[OtherPartyId] LIKE '%{1}%' OR
                                                  cj.[OtherParty] LIKE '%{1}%' OR
                                                  cj.[OtherPartyCountry] LIKE '%{1}%' OR
                                                  cj.[EquipmentCode] LIKE '%{1}%' OR
                                                  cj.[Equipment] LIKE '%{1}%' OR
                                                  cj.[Ref] LIKE '%{1}%' OR
                                                  cj.[OtherRef] LIKE '%{1}%' OR
                                                  cj.[BatchRef] LIKE '%{1}%' OR
                                                  cj.[InvoiceNumber] LIKE '%{1}%' OR
                                                  cj.[DataSource] LIKE '%{1}%' OR
                                                  cj.[CreatedBy] LIKE '%{1}%' OR
                                                  cj.[Quantity] LIKE '%{1}%' OR
                                                  c.[Email] LIKE '%{1}%' OR
                                                  c.[AdminEmail] LIKE '%{1}%' OR
                                                  c.[AdminPerson] LIKE '%{1}%' OR
                                                  c.[CompanyName] LIKE '%{1}%' OR
                                                  c.[ChepReference] LIKE '%{1}%' OR
                                                  c.[CompanyRegistrationNumber] LIKE '%{1}%' OR
                                                  r.[Description] LIKE '%{1}%'
                                                 ) ", query, csm.Query.Trim() );
            }

            #endregion

            // ORDER

            query = $"{query} ORDER BY {( pm.SortBy.Contains( "." ) ? pm.SortBy : "cj." + pm.SortBy )} {pm.Sort}";

            // SKIP, TAKE

            query = string.Format( "{0} OFFSET (@skip) ROWS FETCH NEXT (@take) ROWS ONLY ", query );

            List<ChepLoadJournalCustomModel> model = context.Database.SqlQuery<ChepLoadJournalCustomModel>( query, parameters.ToArray() ).ToList();

            return model;
        }
    }
}
