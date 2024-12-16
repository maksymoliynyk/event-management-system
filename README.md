# Event Management System

This is a simple event management system that allows users to create, delete and update events, invite attendees, manage RSVPs and check the history of created events. The system is built using the ASP.NET framework and uses a SQL Server database.

## User stories

### Event owner

- As an event owner, I want to be able to create an event.
- As an event owner, I want to be able to delete an event.
- As an event owner, I want to be able to update information about event.
- As an event owner, I want to be able to invite attendees to an event.
- As an event owner, I want to be able to see my created events.
- As an event owner, I want to be able to see sent invitations and their statuses.
- As an event owner, I want to be able to see attendees.

### Attendee

- As an attendee, I want to see events that I'm going to attend.
- As an attendee, I want to be able to manage invitations.

## Api Endpoints

### Authentication

`POST /auth/register`
Register a new user.

`POST /auth/login`
Log in as a user.

### Event Management (Event Owner)

`POST /events`
Create a new event.

`GET /events?owner=true`
Retrieve all events created by the authenticated user (event owner).

`GET /events/{id}`
Retrieve detailed information about a specific event.

`PATCH /events/{id}/cancel`
Cancel a specific event

`!!! not implemented`
`PUT /events/{id}`
Update information about a specific event (e.g., time, location).

`DELETE /events/{id}`
Delete a specific event created by the authenticated user.

`POST /events/{id}/invites`
Send invitations to attendees for a specific event.

`GET /events/{id}/invites`
Retrieve the list of invitations sent for a specific event and their statuses (e.g., accepted, declined, pending).

`GET /events/{id}/attendees`
Retrieve a list of attendees who have accepted the invitation to a specific event.

### Event Management (Attendee)

`GET /events`
Retrieve a list of events the authenticated user is attending.

`PATCH /events/{eventId}/invites`
Respond to a specific invitation (e.g., accept, decline, or mark as tentative).

`GET /invites`
Retrieve a list of received invitations for the authenticated user.
