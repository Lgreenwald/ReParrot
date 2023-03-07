import React from "react";
import debug from "sabio-debug";
import PropTypes from 'prop-types';
import { Card, Col, Image } from 'react-bootstrap';

function OrgDashBlogs(props) {
    const _logger = debug.extend("BlogsCard");

    const blog = props.blog

    _logger("blog", blog)

    return (
        <Card>
            <Card.Body className="org-dash-blog-card">
                <Col className="col-auto col">
                    <Image
                        src={blog?.author?.avatarUrl}
                        className="avatar avatar-md avatar-indicators avatar-online rounded-circle"
                        alt=""
                    />
                </Col>
                <Col className="org-dash-blog-text">
                    <h5>{blog.author.firstName}{blog.author.lastName}</h5>
                    <p className="mb-1">{blog.title}</p>
                </Col>
            </Card.Body>
        </Card>
    );
};

OrgDashBlogs.propTypes = {
    blog: PropTypes.shape({
        author: PropTypes.shape({
            firstName: PropTypes.string.isRequired,
            lastName: PropTypes.string.isRequired,
            avatarUrl: PropTypes.string.isRequired,
        }),
        title: PropTypes.string.isRequired,
    }),
};

export default OrgDashBlogs;