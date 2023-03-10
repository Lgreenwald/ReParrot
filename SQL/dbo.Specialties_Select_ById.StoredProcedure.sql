USE [ReParrot]
GO
/****** Object:  StoredProcedure [dbo].[Specialties_Select_ById]    Script Date: 3/7/2023 3:34:18 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:	Lauren Greenwald
-- Create date: 01/25/2023
-- Description:	Specialties Select By Id
-- Code Reviewer: Kathy Xayasomloth


-- MODIFIED BY: author
-- MODIFIED DATE:
-- Code Reviewer: 
-- Note: 
-- =============================================

CREATE proc [dbo].[Specialties_Select_ById]
			
			@Id int

AS

/*-----------------------TEST CODE-----------------------
	DECLARE @Id int = 1

	EXECUTE dbo.Specialties_Select_ById @Id
*/

BEGIN

	SELECT	  s.Id 
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

	From dbo.Specialties as s
		INNER JOIN dbo.Organizations as o
			ON s.OrganizationId = o.Id

	WHERE s.Id = @Id

END
GO
