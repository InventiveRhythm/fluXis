---@meta

---@class mathf
---@field pi number
mathf = {};

---@param d number
---@return number
---@nodiscard
function mathf:cos(d) end

---@param x number
---@param y number
---@return number
---@nodiscard
function mathf:atan2(x, y) end

---@param x Vector2
---@param y Vector2
---@return Vector2
---@nodiscard
function mathf:vecsub(x, y) end

return mathf
