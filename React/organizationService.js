import axios from "axios";
import * as serviceHelpers from "./serviceHelpers";
import debug from "sabio-debug";

const _logger = debug.extend("OrganizationService");

const endpoint = `${serviceHelpers.API_HOST_PREFIX}/api/organizations`;

const createOrganization = (payload) => {
  _logger("orgService", payload);
  const config = {
    method: "POST",
    url: endpoint,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const getOrganization = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url: `${endpoint}/paginate/?pageIndex=${pageIndex}&&pageSize=${pageSize}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const getAllOrganizationValid = (pageIndex, pageSize) => {
  const config = {
    method: "GET",
    url: `${endpoint}/paginate/valid?pageIndex=${pageIndex}&&pageSize=${pageSize}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const getAllOrganizationNoPag = () => {
  const config = {
    method: "GET",
    url: `${endpoint}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const deleteOrganization = (orgId) => {
  const config = {
    method: "DELETE",
    url: `${endpoint}/${orgId}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config);
};

const editOrganization = (payload) => {
  const config = {
    method: "PUT",
    url: `${endpoint}/loc/${payload.id}`,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config);
};

const searchOrganizations = (pageIndex, pageSize, query) => {
  const config = {
    method: "GET",
    url: `${endpoint}/search?pageIndex=${pageIndex}&pageSize=${pageSize}&query=${query}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const searchValidOrganizations = (pageIndex, pageSize, query) => {
  const config = {
    method: "GET",
    url: `${endpoint}/search/valid?pageIndex=${pageIndex}&pageSize=${pageSize}&query=${query}`,
    withCredentials: true,
    crossdomain: true,
    hearders: { "Content-Type": "application/json" },
  };

  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const getByIdOrganization = (orgId) => {
  const config = {
    method: "GET",
    url: `${endpoint}/${orgId}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const getOrgByUser = () => {
  const config = {
    method: "GET",
    url: `${endpoint}/current`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const editOrgValidation = (isValidated, orgId) => {
  const config = {
    method: "PUT",
    url: `${endpoint}/${orgId}/validation?isValidated=${isValidated}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const getUsersByOrgId = (pageIndex, pageSize, orgId) => {
  const config = {
    method: "GET",
    url: `${endpoint}/orgid/?pageIndex=${pageIndex}&pageSize=${pageSize}&orgid=${orgId}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const deleteMemberById = (id) => {
  const config = {
    method: "DELETE",
    url: `${endpoint}/orgMember/${id}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const getByUserIdV4 = (orgId) => {
  _logger("getByUserIdV4", orgId);
  const config = {
    method: "GET",
    url: `${endpoint}/users/${orgId}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const updateOrgMemberRoles = (payload) => {
  const config = {
    method: "PUT",
    url: `${endpoint}/orgmember/roles`,
    withCredentials: false,
    data: payload,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

const organizationService = {
  createOrganization,
  getOrganization,
  getAllOrganizationValid,
  getAllOrganizationNoPag,
  deleteOrganization,
  editOrganization,
  searchOrganizations,
  getByIdOrganization,
  getUsersByOrgId,
  getOrgByUser,
  searchValidOrganizations,
  getByUserIdV4,
  editOrgValidation,
  deleteMemberById,
  updateOrgMemberRoles
};

export default organizationService;
