---@meta

---@class map
map = {}

---Get all or specific types of Hitobjects in a range. Leave type parameter if you want all HitObjects.
---'Normal' HitObjectType will also give long notes not just normal notes.
---@param startTime number
---@param endTime number
---@param type HitObjectType?
---@return table
---@nodiscard
function map:NotesInRange(startTime, endTime, type) end

---@param startTime number
---@param endTime number
---@return table
---@nodiscard
function map:TimingPointsInRange(startTime, endTime) end

---@param startTime number
---@param endTime number
---@return table
---@nodiscard
function map:ScrollVelocitiesInRange(startTime, endTime) end

---@param startTime number
---@param endTime number
---@return table
---@nodiscard
function map:HitSoundFadesInRange(startTime, endTime) end

---@param startTime number
---@param endTime number
---@param eventType EventType
---@return table
---@nodiscard
function map:EventsInRange(startTime, endTime, eventType) end