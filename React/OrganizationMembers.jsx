import React, { useState, useEffect } from "react";
import debug from "sabio-debug";
import "./organization.css";
import Pagination from 'rc-pagination';
import { Col, Row, Figure, Container } from "react-bootstrap";
import organizationService from "../../services/organizationService";
import OrgMember from "./OrgMemberCard";
import PropTypes from "prop-types";
import locale from "rc-pagination/lib/locale/en_US";
import "rc-pagination/assets/index.css";

const _logger = debug.extend("organization Members")

function OrganizationMembers(props) {
  const currentUser = props.currentUser;

  _logger("current user", currentUser)

  const [memberData, setMemberData] = useState({
    arrayOfMembers: [],
    memberComponents: [],
    organizationData: {},
  });

  const [paginate, setPaginate] = useState({
    pageIndex: 0,
    pageSize: 4,
    totalCount: 0,
  });

  useEffect(() => {
    organizationService.getUsersByOrgId(0, 10, currentUser.organizationId).then(onGetMembersSuccess).catch(onGetMembersError);
    organizationService.getByIdOrganization(currentUser.organizationId).then(onGetOrgSuccess).catch(onGetOrgError);
  }, []);

  const onGetOrgSuccess = (response) => {
    _logger("get org data success", response)
    setMemberData((prevState) => {
      const memberData = { ...prevState };
      memberData.organizationData = response.item;
      return memberData;
    })
  }

  const onGetOrgError = (response) => {
    _logger("get org data error", response)
  }

  const onGetMembersSuccess = (response) => {
    let arrayOfMembers = response.item.pagedItems;
    _logger("array of members", arrayOfMembers);

    setMemberData((prevState) => {
      const memberData = { ...prevState };
      memberData.arrayOfMembers = arrayOfMembers;
      memberData.memberComponents = arrayOfMembers.map(mapOrgMember)
      _logger("member data", memberData)
      return memberData;
    });

    setPaginate((prevState) => {
      const newPageData = {
        ...prevState,
        total: response.item.totalCount,
      };
      return newPageData;
    });
  };

  const onPageChange = (pageNumber) => {
    setPaginate((prevState) => {
      const updatedPage = { ...prevState, index: pageNumber - 1 };
      return updatedPage;
    });
  };

  const onGetMembersError = (response) => {
    _logger("error get member", response);
  };

  const onLocalDeleteRequest = (deleteMember) => {
    _logger("deleted member data", deleteMember)
    setMemberData((prevState) => {
      const pd = { ...prevState };
      pd.arrayOfMembers = [...pd.arrayOfMembers];

      const indexOf = pd.arrayOfMembers.findIndex((member) => {
        let result = false;
        if (member.id === deleteMember.id) {
          result = true;
        }
        return result;
      });
      if (indexOf >= 0) {
        pd.arrayOfMembers.splice(indexOf, 1);
        pd.memberComponents = pd.arrayOfMembers.map(mapOrgMember);
      }
      return pd;
    })
  }

  const mapOrgMember = (aMember) => {
    _logger("mapping", aMember)
    return (
      <Col lg={3} md={6} sm={12} key={aMember.id}>
        <OrgMember
          member={aMember}
          key={"MemberList" + aMember.id}
          currentUser={props.currentUser}
          onDeleteRequested={onLocalDeleteRequest}
        />
      </Col>
    );
  };

  return (
    <React.Fragment>
      <div className="org-dashboard-header center-object-dashboard mb-4 pb-0 d-lg-flex align-items-center">
        <Figure className="mt-4 mb-4">
          <img
            className="dashboard-header-image"
            alt=""
            src={memberData?.organizationData?.logoUrl} />
        </Figure>
      </div>
      <Container>
        <Row>
          <Pagination
            onChange={onPageChange}
            index={paginate.index + 1}
            total={paginate.total}
            pageSize={paginate.pageSize}
            totalCount={paginate.totalCount}
            locale={locale}
            className="org-member-pagination justify-content-center mb-0"
          />
        </Row>
        <Row className="mb-3 org-member-card">
          {memberData?.memberComponents}
        </Row>
      </Container>
    </React.Fragment>
  );
};

OrganizationMembers.propTypes = {
  currentUser: PropTypes.shape({
    id: PropTypes.number.isRequired,
    firstName: PropTypes.string,
    mi: PropTypes.string,
    lastName: PropTypes.string.isRequired,
    email: PropTypes.string.isRequired,
    isLoggedIn: PropTypes.bool.isRequired,
    roles: PropTypes.arrayOf(PropTypes.string),
    organizationId: PropTypes.number.isRequired
  }),
  roles: PropTypes.arrayOf(PropTypes.string),
};

export default OrganizationMembers