import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { Col, Row, Nav, Container, Figure, Card, Carousel } from "react-bootstrap";
import debug from "sabio-debug";
import organizationService from "../../services/organizationService";
import ratingsService from "../../services/ratingsService";
import "./organization.css";
import ApexCharts from "../dashboard/analytics/ApexCharts";
import newsLetter from "../landing/newsLetter";
import {
  AppointmentsColumnChartOptions,
  AppointmentsColumnChartSeries,
} from "./ChartData";
import toastr from "toastr";
import PropTypes from 'prop-types';
import { Users, Star } from "react-feather";
import appointmentService from "../../services/appointmentService";
import blogsService from "../../services/blogsService";
import OrgDashBlogs from "./OrgDashBlogs"
import OrgDashAppts from "./OrgDashAppts"


const _logger = debug.extend("OrgAdmin");

const OraganizationDashboard = (props) => {

  const currentUser = props.currentUser

  _logger("current User")

  // const [pageData, setPageData] = useState({ appts: [], apptsComponents: [] })

  useEffect(() => {
    organizationService
      .getByIdOrganization(currentUser.organizationId)
      .then(onGetOrgSuccess)
      .catch(onGetOrgError);
    ratingsService.getAverageRating({ "entityId": currentUser.organizationId, "entityTypeId": 7 })
      .then(getRatingSuccess)
      .catch(getRatingError);
    organizationService
      .getUsersByOrgId(0, 10, currentUser.organizationId)
      .then(getAllOrgMembersSuccess)
      .catch(getAllOrgMembersError)
    appointmentService
      .getAllByOrgId(0, 10, currentUser.organizationId)
      .then(getApptsByOrgIdSuccess)
      .catch(getApptsByOrgIdError)
    blogsService
      .getBlog(0, 3)
      .then(onGetBlogsuccess)
      .catch(onGetBlogError)
  }, []);

  const [theOrg, setOrg] = useState({
    anOrg: {},
    rating: 0,
    memberCount: 0,
    blogs: [],
    blogsComponents: [],
    appts: [],
    apptsComponents: [],
  });

  _logger("current org", theOrg.anOrg)

  const onGetOrgSuccess = (response) => {
    _logger(response);
    let currentOrg = response.item;

    setOrg((prevState) => {
      const theOrg = { ...prevState };
      theOrg.anOrg = currentOrg;
      return theOrg;
    });
  };

  const onGetOrgError = (err) => {
    _logger(err);
    toastr.error("Get Org fail");
  };

  const getRatingSuccess = (response) => {

    let orgRating = response.item;

    setOrg((prevState) => {
      const theOrg = { ...prevState };
      theOrg.rating = orgRating;
      return theOrg;
    });
  };

  const getRatingError = (error) => {
    _logger("get rating error", error)
  };

  const getAllOrgMembersSuccess = (response) => {

    const totalCount = response.item.totalCount;

    setOrg((prevState) => {
      const theOrg = { ...prevState };
      theOrg.memberCount = totalCount;
      return theOrg;
    });
  };

  const getAllOrgMembersError = (error) => {
    _logger("get all org members error", error)
  };

  const getApptsByOrgIdSuccess = (data) => {
    let arrOfAppts = data.item.pagedItems;

    setOrg((prevState) => {
      const pd = { ...prevState };
      pd.appts = arrOfAppts;
      pd.apptsComponents = arrOfAppts.map(mapAppointment);
      return pd;
    });
  };

  const getApptsByOrgIdError = (error) => {
    _logger("get all appointments error", error)
  };

  const mapAppointment = (anAppt) => {
    return (
        <OrgDashAppts
          appt={anAppt}
          orgId={anAppt.organization.id}
          // currentUser={props.currentUser}
        />
    );
  };

  const onGetBlogsuccess = (response) => {
    _logger("get blog success", response.item.pagedItems)


    setOrg((prevState) => {
      const theOrg = { ...prevState };
      theOrg.blogs = response.item.pagedItems;
      theOrg.blogsComponents = response.item.pagedItems.map(mapBlog);
      return theOrg;
    });
  };

  const mapBlog = (aBlog) => {
    return (
      <OrgDashBlogs
        blog={aBlog}
        key={aBlog.id}
      />
    );
  };

  const onGetBlogError = (error) => {
    _logger("get blog error", error)
  }

  const slide1 = newsLetter.find((obj) => obj.id === 1);
  const slide2 = newsLetter.find((obj) => obj.id === 2);
  const slide3 = newsLetter.find((obj) => obj.id === 3);

  return (
    <React.Fragment>
      <Container>
        <Row>
          <Col className="org-dashboard-header center-object-dashboard mb-4 pb-0 d-lg-flex align-items-center">
            <Figure className="mt-4 mb-4">
              <img
                className= "dashboard-header-image"
                alt="Logo here"
                src={theOrg?.anOrg?.logoUrl}
              />
            </Figure>
          </Col>
        </Row>
      </Container>
      <Container>
        <Row>
          <Card>
            <Nav className="nav" defaultActiveKey="/#" as="ul">
              <Col>
                <li className="nav-item center-object-dashboard">
                  <Link to="/#" className="nav-link link-button">
                    Home
                  </Link>
                </li>
              </Col>
              <Col>
                <li className="nav-item center-object-dashboard">
                  <Link to="/autoservices/add" className="nav-link link-button">
                    Add Service
                  </Link>
                </li>
              </Col>
              <Col>
                <li className="nav-item center-object-dashboard">
                  <Link
                    to="/appointments"
                    className="nav-link link-button"
                  >
                    Appointments
                  </Link>
                </li>
              </Col>
              <Col>
                <li className="nav-item center-object-dashboard">
                  <Link
                    to="/schedules/view"
                    className="nav-link link-button"
                  >
                    Schedules
                  </Link>
                </li>
              </Col>
                    <Col>
                <li className="nav-item center-object-dashboard">
                  <Link
                    to="/links"
                    className="nav-link link-button"
                  >
                    Social Media
                  </Link>
                </li>
              </Col>
            </Nav>
          </Card>
        </Row>
        <Row className="top-space-dashboard ">
          <Col xl={4} lg={4} md={12} sm={12}>
            <Card className="org-dash-members">
              <Card.Body>
                <h3>Organization Members</h3>
                <Users
                  size="22px"
                  className="user-item-icon-org-dash"
                ></Users>
                <h2 className="org-dashboard-stats-numbers">{theOrg?.memberCount}</h2>
                <Link to="/organization/memberlist" className="nav-link link-button">
                  Edit Members
                </Link>
              </Card.Body>
            </Card>
            <Card className="org-service-rating">
              <Card.Body>
                <h3>Service Rating</h3>
                <Star
                  size="22px"
                  className="user-item-icon-org-dash"
                  fill="#FFD700"
                  stroke="#FFD700"
                />
                <h2 className="org-dashboard-stats-numbers">
                  {theOrg.rating.toFixed(2)}
                </h2>
              </Card.Body>
            </Card>
          </Col>
          <Col xl={4} lg={4} md={12} sm={12}>
            <Card className="org-dash-appointments-this-week">
              <Card.Body>
                <h3>Appointments this Week</h3>
                <ApexCharts
                  options={AppointmentsColumnChartOptions}
                  series={AppointmentsColumnChartSeries}
                  height={287}
                  type="bar"
                />
              </Card.Body>
            </Card>
          </Col>
           <Col xl={4} lg={4} md={12} sm={12}>
            <Card  className="org-dash-upcoming-appointments">
             <Card.Header>
                <h3>Upcoming Appointments</h3>
                  <Link to="/appointments" className="nav-link link-button">
                  View All Appointments
                </Link>
              </Card.Header>
              <Card.Body className=".org-dash-appointments-card">
              {theOrg.apptsComponents}
              </Card.Body>
            </Card>
          </Col>
        </Row>
        <Row className="org-dashboard-bottom">
          <Col xl={6} lg={6} md={12} sm={12} >
            <Card className="h-100" >
              <Card.Header>
                <h3>Blogs</h3>
              </Card.Header>
              <Card.Body>
                {theOrg.blogsComponents}
              </Card.Body>
              <Card.Footer>
                <Link to="/blogs" className="nav-link link-button">
                  View More Blogs
                </Link>
              </Card.Footer>
            </Card>
          </Col>
          <Col xl={6} lg={6} md={12} sm={12}>
            <Card className="h-100">
              <Card.Header>
                <h3>Newsletters</h3>
              </Card.Header>
              <Card.Body>
              <Carousel>
                <Carousel.Item>
                  <img
                    className="d-block w-100"
                    src={slide1.mainImg}
                    alt="slide here"
                  />
                  <Carousel.Caption>
                    <h3 className="color-white-dashboard">{slide1.title}</h3>
                    <p>{slide1.content}</p>
                  </Carousel.Caption>
                </Carousel.Item>
                <Carousel.Item>
                  <img
                    className="d-block w-100"
                    src={slide2.mainImg}
                    alt="First slide"
                  />
                  <Carousel.Caption>
                    <h3 className="color-white-dashboard">{slide2.title}</h3>
                    <p>{slide2.content}</p>
                  </Carousel.Caption>
                </Carousel.Item>
                <Carousel.Item>
                  <img
                    className="d-block w-100"
                    src={slide3.mainImg}
                    alt="First slide"
                  />
                  <Carousel.Caption>
                    <h3 className="color-white-dashboard">{slide3.title}</h3>
                    <p>{slide3.content}</p>
                  </Carousel.Caption>
                </Carousel.Item>
                </Carousel>
                </Card.Body>
              <Card.Footer>
                <Link to="/newsletters" className="nav-link link-button">
                  View More Newsletters
                </Link>
                </Card.Footer>
            </Card>
          </Col>
        </Row>
      </Container>
    </React.Fragment>
  );
};

OraganizationDashboard.propTypes = {
  currentUser: PropTypes.shape({
    organizationId: PropTypes.number.isRequired,
  }),
};

export default OraganizationDashboard;
