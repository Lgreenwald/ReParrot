import React, {
  Fragment, useState, useEffect
} from 'react';
import PropTypes from 'prop-types';
import { Edit } from "react-feather";
import { Form, Col, Card, Image, Row, Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'react-bootstrap';
import Select from "react-select";
import { Formik, Form as FormikForm, Field } from "formik";
import debug from "sabio-debug";
import userService from "../../services/userService";
import organizationService from "../../services/organizationService";
import toastr from "toastr";
import Swal from "sweetalert2";


const _logger = debug.extend("OrganizationMemberCard");

function OrgMemberCard(props) {

  const member = props.member;
  const currentUser = props.currentUser;

  const [modalState, setModalState] = useState(false);
  const [checked, setChecked] = useState(true);

  const formData = {
    roles: "",
    isActive: true,
  };

  const [roles, setRoles] = useState({
    originalRoles: [],
    mappedRoles: [],
    rolesForUpdate: [],
  });

  useEffect(() => {
    const allMappedRoles = userService.getAllRoles().then(onGetRolesSuccess).catch(onGetRolesError)
    _logger("mapped roles", allMappedRoles)
  }, [])

  const onGetRolesSuccess = (response) => {
    var initialRoles = response
    _logger("get roles success", response);

    setRoles((prevState) => {
      const roles = { ...prevState };
      roles.originalRoles = initialRoles;
      roles.mappedRoles = initialRoles.item.map(mapRoles);
      return roles;
    })
  };

  const mapRoles = (aRole) => {
    return { label: aRole.name, value: aRole.id };
  };

  const onGetRolesError = (error) => {
    _logger("get roles error", error)
    toastr.error("Error retrieving roles");
  };

  const handleSubmit = (values) => {
    if (currentUser.organizationId === member.orgId) {
      if (values.roles.length === 0) {
        Swal.fire({
          title: "Members must have at least one role, please verify roles and try again.",
          icon: "warning",
          confirmButtonText: "Close"
        })
      }
      else {
        const rolestoMap = {
          ...values,
        }
        roles.rolesForUpdate = rolestoMap.roles.map(mapRoleUpdates);
        organizationService.updateOrgMemberRoles({ "RoleIds": roles.rolesForUpdate, "UserToUpdateId": member.userId }).then(onRoleUpdateSuccess).catch(onRoleUpdateError)
        _logger("id to update", member.userId)
      }
    }
    else {
      Swal.fire({
        title: "You can only modify roles of members within your organization.",
        icon: "warning",
        confirmButtonText: "Close"
      })
    };
  };

  const onRoleUpdateSuccess = () => {
    toastr.success("Roles updated successfully!");
  };

  const onRoleUpdateError = (error) => {
    _logger("role update error", error)
    toastr.error("Error updating roles");
  };

  const toggleModal = () => {
    setModalState(!modalState);
  };

  const onRoleSelectChange = (value, setFieldValue) => {
    setFieldValue(
      "roles",
      value
    );
  };

  const handleChange = () => {
    Swal.fire({
      title: "This will remove this member from the organization, are you sure you would like to do this?",
      icon: "warning",
      showDenyButton: true,
      confirmButtonText: "Remove Member",
      denyButtonText: "Cancel"
    }).then((result) => {
      if (result.isConfirmed) {
        _logger("member id", member.userId)
         organizationService.updateOrgMemberRoles({ "RoleIds": [4], "UserToUpdateId": member.userId }).then(onUpdateToDeleteSuccess).catch(onUpdateToDeleteError)
       
        setChecked(!checked);
      }
      else if (result.isDenied) {
        Swal.fire("Changes not saved")
      };
    });
  };

  const onUpdateToDeleteSuccess = (response) => {
    _logger("update to delete success", response)
    organizationService.deleteMemberById(member.id).then(onDeleteMemberByIdSuccess).catch(onDeleteMemberByIdError)
  }
  
  const onUpdateToDeleteError = (response) => {
    _logger("update to delete error", response)
  }

  const onDeleteMemberByIdSuccess = (response) => {
    Swal.fire("Organization Member Deactivated")
    props.onDeleteRequested(member, response);
  };

  const onDeleteMemberByIdError = (response) => {
    _logger("delete error", response)
    Swal.fire("Error removing member, please try again")
  };

  const mapRoleUpdates = (aRole) => {
    return aRole.value;
  };

  const isOrgAdmin = member?.roles.findIndex((role) => role.name === "OrgAdmin") > -1

  return (
    <Fragment>
      <Card className="mb-4 org-member-card card-hover p-2 text-center" >
        <Image
          src={member?.avatarUrl}
          className="org-member-card-img-top rounded-top-md img-fluid"
          alt=""
        />
        <Card.Body className="mb-2 p-2">
          <h5 className="card-title h4 mb-2 text-primary">{member?.firstName} {member?.mi} {member?.lastName}
          </h5>
          <p>{isOrgAdmin ? "OrgAdmin" : null}</p>
        </Card.Body>
        <Card.Footer className="org-member-card-footer">
          <Row className="align-items-center">
            <Col className="col ms-2">
              {
                member.userId !== props.currentUser.id ?
                  (props.currentUser.roles.includes("SysAdmin") ||
                    props.currentUser.roles.includes("OrgAdmin")) && (
                    <Edit
                      size="22px"
                      className="edit-item-icon-orgMemberCard"
                      type="button"
                      onClick={toggleModal}
                    ></Edit>
                  ) : ("Current User")
              }
            </Col>
          </Row>
        </Card.Footer>
      </Card>
      <Modal show={modalState} toggle={toggleModal}>
        <ModalHeader className="card-title h4 mb-2 text-truncate-line-2 text-primary">{member.firstName} {member.mi} {member.lastName}</ModalHeader>
        <ModalBody>
          <Formik
            enableReinitialize={true}
            initialValues={formData}
            onSubmit={handleSubmit}
          >
            {({ setFieldValue }) => (
              <FormikForm>
                <Form.Label>Roles</Form.Label>
                <Select
                  defaultValue={member?.roles?.map(role => ({ label: role.name, value: role.id }))}
                  isMulti
                  name="mappedRoles"
                  options={roles?.mappedRoles}
                  className="basic-multi-select"
                  classNamePrefix="select"
                  onChange={(data) =>
                    onRoleSelectChange(data, setFieldValue)
                  }
                />
                <Field type="checkbox" id="isActive" name="isActive" checked={checked} onChange={handleChange} />
                <Form.Label htmlFor="isActive" className="p-2">
                  Is this person Active?
                </Form.Label>
                <ModalFooter>
                  <Button color="secondary" type="submit">
                    Submit
                  </Button>
                  <Button color="secondary" onClick={toggleModal}>
                    Close
                  </Button>
                </ModalFooter>
              </FormikForm>
            )}
          </Formik>
        </ModalBody>
      </Modal>
    </Fragment>
  );
};

OrgMemberCard.propTypes = {
  member: PropTypes.shape({
    id: PropTypes.number.isRequired,
    avatarUrl: PropTypes.string,
    firstName: PropTypes.string.isRequired,
    mi: PropTypes.string,
    lastName: PropTypes.string.isRequired,
    roles: PropTypes.arrayOf(PropTypes.shape({
      id: PropTypes.number,
      name: PropTypes.string,
    })),
    positionName: PropTypes.string,
    userId: PropTypes.number.isRequired,
    orgId: PropTypes.number.isRequired
  }),
  currentUser: PropTypes.shape({
    id: PropTypes.number.isRequired,
    email: PropTypes.string.isRequired,
    isLoggedIn: PropTypes.bool.isRequired,
    roles: PropTypes.arrayOf(PropTypes.string),
    organizationId: PropTypes.number.isRequired,
  }),
  onDeleteRequested: PropTypes.func
};

export default OrgMemberCard;

