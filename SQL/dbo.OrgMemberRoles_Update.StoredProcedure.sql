USE [ReParrot]
GO
/****** Object:  StoredProcedure [dbo].[OrgMemberRoles_Update]    Script Date: 3/7/2023 3:34:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Lauren Greenwald
-- Create date: 02/xx/2023
-- Description:	used to update user roles
-- Code Reviewer: 


-- MODIFIED BY: 
-- MODIFIED DATE:
-- Code Reviewer: 
-- Note: 
-- =============================================
CREATE proc [dbo].[OrgMemberRoles_Update]
			@UserToUpdateId int
			,@AuthorizedUserId int
			,@RoleId int

as

/*-----------------TEST CODE---------------------
	DECLARE	@UserId int = 159
			,@AuthorizedUserId int = 139
			,@RoleId int = 3

			--select * 
			--	from dbo.UserRoles
			--	Where UserId=@UserId

	EXECUTE	dbo.OrgMemberRoles_Update
				@UserId
			,@AuthorizedUserId 
				,@RoleId
			
				select * 
				from dbo.UserRoles
				Where UserId=@UserId

						select * 
				from dbo.Roles

	SELECT *
	FROM dbo.OrganizationMembers as om
	INNER JOIN dbo.UserRoles AS ur on ur.userid = om.userid
	INNER JOIN dbo.Roles AS r on r.Id = ur.roleid
	ORDER BY om.OrganizationId, om.userId

	--SELECT * FROM Dbo.Roles

*/

BEGIN

	DECLARE @ORgCount INT = 0
	SELECT 
		@ORgCount= count(*) OVER()
	FROM dbo.OrganizationMembers as om
	WHERE @UserToUpdateId = om.UserId OR @AuthorizedUserId = om.UserId
	group by om.OrganizationId

	SELECT @ORgCount

	--UPDATE	[dbo].[UserRoles]
	--SET		[RoleId]= @RoleId

	--WHERE UserId = @UserToUpdateId

END

GO
