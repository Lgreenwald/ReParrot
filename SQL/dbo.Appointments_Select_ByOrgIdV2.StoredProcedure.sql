USE [ReParrot]
GO
/****** Object:  StoredProcedure [dbo].[Appointments_Select_ByOrgIdV2]    Script Date: 3/7/2023 3:34:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:	Lauren Greenwald
-- Create date: 03/06/2023
-- Description:	 Appointments for Org Dashboard by Org Id V2
-- Code Reviewer: Jason Spoon


-- MODIFIED BY:
-- MODIFIED DATE: 
-- Code Reviewer:
-- Note:
-- =============================================


CREATE PROC [dbo].[Appointments_Select_ByOrgIdV2]

	@PageIndex int
	,@PageSize int
	,@OrgId int 

as

/*---------------TEST CODE------------------

	Declare @PageIndex int = 0
			,@PageSize int = 10
			,@OrgId int = 161

	Execute [dbo].[Appointments_Select_ByOrgIdV2] 
											@PageIndex
											,@PageSize
											,@OrgId

*/

BEGIN

Declare	@offset int = @PageIndex * @PageSize

		SELECT	appts.Id
				,appts.Notes
				,scheds.Id as ScheduleId
				,scheds.Name as ScheduleName
				,scheds.Description as ScheduleDesc
				,orgs.Id as ScheduleOrgId
				,orgs.Name as ScheduleOrgName
				,servs.Id as ServiceId
				,servs.Name as ServiceName
				,servs.SKU as ServiceSku
				,servs.Description as ServiceDesc
				,servs.UnitCost as ServiceUnitCost
				,locs.Id as LocationId
				,locs.LineOne as LocationLineOne
				,locs.LineTwo as LocationLineTwo
				,locs.City as LocationCity
				,states.Id as LocationStateId
				,states.Name as LocationStateName
				,locs.Zip as LocationZip
				,u.Id as CustomerId
				,u.FirstName as CustomerFirstName
				,u.Mi as CustomerMi
				,u.LastName as CustomerLastName
				,u.Email as CustomerEmail
				,u.AvatarUrl as CustomerAvatarUrl
				,uv.Id as UserVehicleId
				,uv.VIN as UserVehicleVin
				,uv.Color as UserVehicleColor
				,uv.LicensePlate as UserVehicleLicensePlate
				,contTypes.Id as ContactTypeId
				,contTypes.Name as ContactTypeName
				,appts.ContactInfo
				,appts.StartDateTime
				,appts.EstimatedDuration
				,appts.IsConfirmed
				,appts.IsCanceled
				,appts.CreatedBy
				,appts.ModifiedBy 
				,appts.DateCreated
				,appts.DateModified 
				,TotalCount = COUNT(1) OVER() 
		FROM [dbo].[Appointments] as appts 
		inner join [dbo].[Schedules] as scheds
				on appts.ScheduleId = scheds.Id
		inner join [dbo].[Services] as servs
				on appts.ServiceId = servs.Id
		inner join [dbo].[Users] as u
				on appts.CustomerId = u.Id
		inner join [dbo].[UserVehicles] as uv
				on appts.UserVehicleId = uv.Id
		inner join [dbo].[ContactTypes] as contTypes
				on appts.ContactTypeId = contTypes.Id
		inner join [dbo].[Locations] as locs
				on appts.LocationId = locs.Id
		inner join [dbo].[Organizations] as orgs
				on locs.Id = orgs.PrimaryLocationId
		inner join [dbo].[States] as states
				on locs.StateId = states.Id
		WHERE	orgs.Id = @OrgId
				AND appts.IsCanceled = 0
		ORDER BY StartDateTime

		OFFSET @offset Rows
		Fetch Next @PageSize Rows ONLY
END
GO
