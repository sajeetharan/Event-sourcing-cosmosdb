function insertEvents(aggregateRootId, versionNumber, events) {
    var context = getContext();
    var collection = context.getCollection();
    var collectionUri = collection.getSelfLink();
    var response = context.getResponse();

    if (!aggregateRootId) {
        throw new Error("The parameter aggregateRootId is missing.");
    }
    if (typeof aggregateRootId !== "string") {
        throw new Error("The parameter aggregateRootId must be a string.");
    }
    if (typeof versionNumber === "string") {
        versionNumber = parseInt(versionNumber);
    }
    if (typeof versionNumber !== "number") {
        throw new Error("The parameter versionNumber must be a number.");
    }
    if (typeof events === "string") {
        events = JSON.parse(events);
    }
    if (!Array.isArray(events)) {
        throw new Error("The parameter events must be an array.");
    }

    var numberOfEvents = events.length;
    if (numberOfEvents === 0) {
        response.setBody(versionNumber);
        return;
    }

    var index = 0;
    tryInsert(events[index]);

    function tryInsert(event) {
        var nextVersionNumber = versionNumber + 1;
        var document = {
            aggregateRootId,
            id: String(nextVersionNumber),
            versionNumber: nextVersionNumber,
            event,
        };
        var isAccepted = collection.createDocument(collectionUri, document, callback);
        if (!isAccepted) {
            throw new Error('createDocument operation was not accepted.');
        }
    }

    function callback(error) {
        if (error) {
            throw error;
        }

        index++;
        versionNumber++;

        if (index >= numberOfEvents) {
            response.setBody(versionNumber);
        } else {
            tryInsert(events[index]);
        }
    }
}