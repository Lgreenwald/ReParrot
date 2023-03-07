USE [ReParrot]
GO
/****** Object:  StoredProcedure [dbo].[OrganizationMembers_Select_ByOrgId]    Script Date: 3/7/2023 3:34:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Lauren Greenwald
-- Create date: 01/30/2023
-- Description:	Organization Members Select By OrgId
-- Code Reviewer: Jason Spoon


-- MODIFIED BY: author
-- MODIFIED DATE:
-- Code Reviewer: 
-- Note: 
-- =============================================

CREATE proc [dbo].[OrganizationMembers_Select_ByOrgId]

			@OrgId int

AS
/*-----------------------TEST CODE-----------------------
	DECLARE @OrgId int = 161

	EXECUTE dbo.OrganizationMembers_Select_ByOrgId @OrgId
*/

BEGIN

	SELECT	om.Id
			,om.OrganizationId 
			,o.Name
			,u.Id as UserId
			,u.FirstName
			,u.Mi
			,u.LastName
			,u.Email
			,u.AvatarUrl
			,r.Id as RoleId
			,r.Name
			,p.Id as PositionTypeId
			,p.Name

	from dbo.OrganizationMembers as om
		INNER JOIN dbo.Organizations as o
		On om.OrganizationId = o.Id
		INNER JOIN dbo.Users as u
		on om.UserId = u.Id
		INNER JOIN dbo.Roles as r
		on r.Id = om.RoleId
		INNER JOIN dbo.PositionTypes as p
		on om.PositionType = p.Id

	where o.Id = @OrgId

END
GO
