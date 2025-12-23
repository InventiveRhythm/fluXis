---@meta

---@param from number
---@param to number
---@return number
---@nodiscard
function RandomRange(from, to) end

---@param time number
---@return number
---@nodiscard
function BPMAtTime(time) end

---@param key string
---@param title string
---@param type ParameterDefinitionType
---@param fallback any?
function DefineParameter(key, title, type, fallback) end

---@param text string
function print(text) end