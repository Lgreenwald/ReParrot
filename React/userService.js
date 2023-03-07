import Axios from "axios";
import * as serviceHelpers from "./serviceHelpers";

let userService = { endpoint: `${serviceHelpers.API_HOST_PREFIX}/api/users` };

userService.add = (payload) => {
  const config = {
    method: "POST",
    url: userService.endpoint,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return Axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

userService.getCurrentUser = () => {
  const config = {
    method: "GET",
    url: `${userService.endpoint}/currentuser`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-type": "application/json" },
  };
  return Axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

userService.login = (payload) => {
  const config = {
    method: "POST",
    url: `${userService.endpoint}/login`,
    data: payload,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return Axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

userService.logout = () => {
  const config = {
    method: "GET",
    url: `${userService.endpoint}/logout`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-type": "application/json" },
  };
  return Axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

userService.getUserById = (id) => {
  const config = {
    method: "GET",
    url: `${userService.endpoint}/${id}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-type": "application/json" },
  };
  return Axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

userService.getAllRoles = () => {
  const config = {
    method: "GET",
    url: `${userService.endpoint}/roles`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };

  return Axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

userService.updateUserStatus = (status, id) => {
  const config = {
    method: "PUT",
    url: `${userService.endpoint}/status/${id}/${status}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return Axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

userService.confirmUserRegistration = (id, token) => {
  const config = {
    method: "PUT",
    url: `${userService.endpoint}/confirm/${id}?token=${token}`,
    withCredentials: true,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return Axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

userService.resetPassword = (id, payload) => {
  const config = {
    method: "PUT",
    url: `${userService.endpoint}/passwordreset/${id}`,
    withCredentials: false,
    data: payload,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return Axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};

userService.addOrgMember = (payload) => {
  const config = {
    method: "POST",
    url: `${userService.endpoint}/addorgmember`,
    data: payload,
    withCredentials: false,
    crossdomain: true,
    headers: { "Content-Type": "application/json" },
  };
  return Axios(config)
    .then(serviceHelpers.onGlobalSuccess)
    .catch(serviceHelpers.onGlobalError);
};
export default userService;
