import React, { /*useEffect, useState*/ } from "react";
import debug from "sabio-debug";
import { Card, Col, Row, /*Nav, Container, Figure,  Carousel*/ } from "react-bootstrap";
import PropTypes from 'prop-types';
import { format } from 'date-fns'

const _logger = debug.extend("OrgAdmin");

function orgDashAppts(props) {

    _logger("OrgDashAppts props", props)

    const appointment = props.appt

    return (
        <Card>
            <Card.Body>
                <Row>
                    <Col className="org-dash-Appt-text">
                        <h5>{appointment.autoService.name}</h5>
                        <p>{appointment.customer.firstName} {appointment.customer.lastName}</p>
                        <p>{format(new Date(appointment.startDateTime), 'Pp')}</p>
                    </Col>
                </Row>
            </Card.Body>
        </Card>
    )
};

orgDashAppts.propTypes = {

    currentUser: PropTypes.shape({
        autoService: PropTypes.shape({
            name: PropTypes.string.isRequired,
        }),
        customer: PropTypes.shape({
            avatarUrl: PropTypes.string,
            firstName: PropTypes.string.isRequired,
            lastName: PropTypes.string.isRequired,
        }),
        startDateTime: PropTypes.string.isRequired
    }),
};

export default orgDashAppts
