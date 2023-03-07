using Microsoft.AspNetCore.Mvc;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Domain.Organizations;
using Sabio.Models.Requests;
using Sabio.Models.Requests.Locations;
using Sabio.Models.Requests.OrganizationMember;
using Sabio.Models.Requests.Organizations;
using System.Collections.Generic;

namespace Sabio.Services
{
    public interface IOrganizationsService
    {
        void Delete(int id, int userId);
        Organization GetById(int id);
        int Add(OrganizationAddRequest model, int userId);
        Paged<Organization> GetAll(int pageIndex, int pageSize);
        List<Organization> GetAllNoPag();
        void Update(OrganizationUpdateRequest model, int userId);
        void UpdateWithLocation (OrganizationLocationUpdateRequest model, int userId);
		Paged<Organization> Search(int pageIndex, int pageSize, string query);
		Organization GetByUserId(int userId);
        List<Organization> GetByUserIdV4(int userId);
        void AddBatchOrgs(List<PricingImport> importList, List<LocationAddRequest> locations, int userId);
        Paged<Organization> SearchValid(int pageIndex, int pageSize, string query);
        void ToggleValidation(bool isValidated, int id, int userId);
        Paged<OrganizationMember> GetUsersByOrgId(int pageIndex, int pageSize, int orgId);
        int AddOrgMember(OrganizationMemberAddRequest model);
        Paged<Organization> GetAllValid(int pageIndex, int pageSize);
        public void DeleteOrgMember(int Id);
        public void UpdateOrgMemberRoles(OrgMemberRoleUpdate model, int currentUserId);
    }

}
