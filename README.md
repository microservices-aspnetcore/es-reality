# Reality
This is the reality service for the Event Sourcing/CQRS sample from the book. Different languages and different groups have various names for this type of service. The role this fills within the CQRS pattern is the Query service. Its main responsibility is to serve up simple requests for information that are optimized for query, ideally from cached or query-optimized data.

In the case of our sample, _reality_ is the current location of all members as indicated by the most recent report of their location. It is important to remember that this service does _not_ expose the entire event store. You cannot get the complete history of all location events, you can _only_ get the most recent location for any given member or the current locations of all members.

In some more extreme cases, you will see a query-only service and a write-only service for the reality service. To keep the samples from the book a little easier to follow, we have not separated these. Also, this particular type of separation often leads to using a database as an integration layer, which is an _absolute_ anti-pattern for cloud native.

# Reality API

|Resource|Method|Description|
|---|---|---|
|/api/reality/members| GET | Return the current state (location) for all members |
|/api/reality/members/{memberId} | GET | Return the current state (location) for a given member |
|/api/reality/members/{memberId} | PUT | Set the current state (location) for a given member |


