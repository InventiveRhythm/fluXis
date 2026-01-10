---@meta

---@class map
map = {}

---@param startTime number
---@param endTime number
---@return table
---@nodiscard
function map:NotesInRange(startTime, endTime) end

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