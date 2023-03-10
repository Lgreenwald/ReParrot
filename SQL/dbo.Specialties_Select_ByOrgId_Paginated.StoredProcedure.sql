USE [ReParrot]
GO
/****** Object:  StoredProcedure [dbo].[Specialties_Select_ByOrgId_Paginated]    Script Date: 3/7/2023 3:34:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Lauren Greenwald
-- Create date: 01/26/2023
-- Description:	Specialties Select By OrgId (Paginated)
-- Code Reviewer: Kathy Xayasomloth


-- MODIFIED BY: author
-- MODIFIED DATE:
-- Code Reviewer: 
-- Note: 
-- =============================================

CREATE proc [dbo].[Specialties_Select_ByOrgId_Paginated]
			@PageIndex int
			,@PageSize int
			,@OrganizationId nvarchar(100)

AS

/*---------------TEST CODE------------------
	Declare @PageIndex int = 0
			,@PageSize int = 2
			,@OrganizationId nvarchar(100) = 1

	Execute [dbo].[Specialties_Select_ByOrgId_Paginated]
			@PageIndex
			,@PageSize
			,@OrganizationId
*/
BEGIN

	Declare @offset int = @PageIndex * @PageSize

	SELECT  s.Id 
			, s.Name
			, s.Description
			, o.Id
			, o.Name
			, o.LogoUrl
			, o.BusinessPhone
			, o.SiteUrl
			, s.CreatedBy
			, s.ModifiedBy
			, s.DateCreated
			, s.DateModified
			,TotalCount = COUNT(1) OVER()

	From dbo.Specialties as s
		INNER JOIN dbo.Organizations as o
			ON s.OrganizationId = o.Id

	WHERE (o.Id LIKE '%' + @OrganizationId + '%')

	ORDER BY s.Name

	OFFSET @offset Rows
		Fetch Next @PageSize Rows ONLY
	
END
GO
