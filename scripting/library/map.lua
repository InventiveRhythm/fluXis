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
---@param effectTypeValue number
---@return table
---@nodiscard
function map:EffectsInRange(startTime, endTime, effectTypeValue) end