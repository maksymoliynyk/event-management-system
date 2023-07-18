# Event Management System
This is a simple event management system that allows users to create, delete and update events, invite attendees, manage RSVPs and check the history of created events. The system is built using the ASP.NET framework and uses a PostgreSQL database.

## User stories
- As a user, I want to be able to create an event.
- As a user, I want to be able to delete an event.
- As a user, I want to be able to update information about event.
- As a user, I want to be able to invite attendees to an event.
- As a user, I want to be able to see my events.
- As a user, I want to be able to see my recevied invitations.
- As a user, I want to be able to answer an recivied invitation.
- As a user, I want to be able to see who has accepted or declined an invitation.

## Api Endpoints
### Auth
- `POST /auth/register` - register user.
- `POST /auth/login` - login user.
### Event
- `POST /events` - create event. 
- `GET /events ` - get all accessible to user events 
- `GET /events/{id}` - get event by id. 
- `DELETE /events/{id}` - delete event by id.
- `PATCH /events/{id}/cancel` - cancel event by id.
- `POST /events/{id}/invite` - invite user to event.
- `GET /events/{id}/invited` - get event's rspvs.
### User
- `GET /user/events/created` - get events created by user.
- `GET /user/events/participated` - get events where user are participated.
- `GET /user` - get user's info.
- `GET /user/rsvps` - get user's rsvps.
### RSVP
- `GET /rsvp/{id}` - get info about rsvp
- `PATCH /rsvp/{id}` - answer to rsvp

## ToDo
- Write tests for controllers
- Add paging and filtering