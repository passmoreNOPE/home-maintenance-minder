# home-maintenance-minder
A simple reminder app for a home maintenance Google calendar

After authenticating, the app reads events off of a Home Maintenance public calendar for the current month.
(I would imagine you have to subscribe to the calendar itself through Google for this to work. The URL for the calendar is what's listed under the EventsResource.ListRequest. It's a public calendar.)

If it finds any events, it will put them all in an email body and send it to you. Just a nice way to remind yourself of the home maintenance tasks that popped up throughout the month.

I imagine this could be expanded to really any calendar you would want to target. Maybe an idea for the future.

Some Notes:
I initially set this up a few years ago, so I'm a little rusty how exactly I got started on this project. There's a local client_secret.json file that is read which I believe you have to generate yourself. I found some instructions here: https://developers.google.com/calendar/api/guides/overview
I wonder, did they used to have a .NET quickstart guide that I used? Not seeing it listed there any more.

I also got the idea to make this because I wanted to translate these home maintenance calendar events into a Google Keep list. At the time Keep didn't have a public API. That appears to have changed, so that may be another enhancement I make.
