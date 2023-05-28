# Event Management System
This is a simple event management system that allows users to create, delete and update events, invite attendees, manage RSVPs and check the history of created events. The system is built using the ASP.NET framework and uses a PostgreSQL database.

## User stories
### MVP:
- As a user, I want to be able to create an event.
- As a user, I want to be able to delete an event.
- As a user, I want to be able to update information about event.
- As a user, I want to be able to invite attendees to an event(at this development phase, it is only a table with attendees without checking RSVPs).
- As a user, I want to be able to see my created events(the events that not happened or deleted).
- As a user, I want to be able to see the history of created events.
### Additional features:
- As a user, I want to be able to see my recevied invitations.
- As a user, I want to be able to answer an recivied invitation.
- As a user, I want to be able to see who has accepted or declined an invitation.
### Stretch goals:
- As a user, I want to have a nice looking web page.
- As a user, I want to be able to log in system using email.
- As a user, I want to have email notifications about coming events.
- As a user, I want to have email notifications about new invitations.

## Api Endpoints
### Event
- `POST /event` - create event.<!-- + -->
- `GET /event/{id}` - get event by id.<!-- + -->
- `DELETE /event/{id}` - delete event by id.<!-- + -->
- `PATCH /event/{id}/cancel` - cancel event by id.<!-- + -->
- `POST /event/{id}/rsvp` - invite user to event.<!-- + -->
- `GET /event/{id}/rsvps` - get event's rspvs.
### User
- `GET /user/events` - get user's events.
- `GET /user/events/history` - get user's history of events.
- `GET /user` - get user's info.
## Development map(in priority order)
1. Create a system, where users can work with events using CRUD operations.
2. Add to system RSPVs management.
3. Create MVC.
4. Create a system, where user can log in using email.
5. Add email notifications.