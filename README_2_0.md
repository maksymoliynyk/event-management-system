# TODO: Replace main README with this file after refactor

# Event Management System

This is a simple event management system that allows users to create, delete and update events, invite attendees, manage RSVPs and check the history of created events. The system is built using the ASP.NET framework and uses a PostgreSQL database.

## User stories

### Event owner

- As a event owner, I want to be able to create an event.
- As a event owner, I want to be able to delete an event.
- As a event owner, I want to be able to update information about event.
- As a event owner, I want to be able to invite attendees to an event.
- As a event owner, I want to be able to see my created events.
- As a event owner, I want to be able to see sended invitations and their statuses.
- As a event owner, I want to be able to see attendees.

### Attendee

- As a attendee, I want to see events that i'm going to attend.
- As a attendee, I want to be able to manage invitations.

## Api Endpoints

### Authentication

`POST /auth/register`
Register a new user.

`POST /auth/login`
Log in as a user.

### Event Management (Event Owner)

`POST /events`
Create a new event.

`GET /events/owner`
Retrieve all events created by the authenticated user (event owner).

`GET /events/{id}`
Retrieve detailed information about a specific event.

`PUT /events/{id}`
Update information about a specific event (e.g., time, location).

`DELETE /events/{id}`
Delete a specific event created by the authenticated user.

`POST /events/{id}/invite`
Send invitations to attendees for a specific event.

`GET /events/{id}/invites`
Retrieve the list of invitations sent for a specific event and their statuses (e.g., accepted, declined, pending).

`GET /events/{id}/attendees`
Retrieve a list of attendees who have accepted the invitation to a specific event.

### Event Management (Attendee)

`GET /events/attending`
Retrieve a list of events the authenticated user is attending.

### Invite Management (Attendee)

`GET /invites`
Retrieve a list of received invitations for the authenticated user.

`PATCH /invites/{id}`
Respond to a specific invitation (e.g., accept, decline, or mark as tentative).
