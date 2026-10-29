---@meta

---@class map
map = {}

---Get all or specific types of Objects in a range. Leave 'types' parameter if you want all Objects.
---Example:
---NotesInRange(0, 1000, { "flux:keys/normal", "flux:keys/long" })
---@param startTime number
---@param endTime number
---@param types table?
---@return table
---@nodiscard
function map:ObjectsInRange(startTime, endTime, types) end

---Get all or specific types of Hitobjects in a range. Leave 'type' parameter if you want all HitObjects.
---'Normal' HitObjectType will also give long notes not just normal notes.
---Use map:ObjectsInRange(...) instead
---@deprecated
---@param startTime number
---@param endTime number
---@param type string?
---@return table
---@nodiscard
function map:NotesInRange(startTime, endTime, type) end

---Use map:ObjectsInRange(...) instead
---@deprecated
---@param startTime number
---@param endTime number
---@return table
---@nodiscard
function map:TimingPointsInRange(startTime, endTime) end

---Use map:ObjectsInRange(...) instead
---@deprecated
---@param startTime number
---@param endTime number
---@return table
---@nodiscard
function map:ScrollVelocitiesInRange(startTime, endTime) end

---Use map:ObjectsInRange(...) instead
---@deprecated
---@param startTime number
---@param endTime number
---@return table
---@nodiscard
function map:HitSoundFadesInRange(startTime, endTime) end

---Use map:ObjectsInRange(...) instead
---@deprecated
---@param startTime number
---@param endTime number
---@param eventType EventType
---@return table
---@nodiscard
function map:EventsInRange(startTime, endTime, eventType) end