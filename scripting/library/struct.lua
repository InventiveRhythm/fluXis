---@meta

---@class Color4
---@field r number
---@field g number
---@field b number
---@field a number
local __Color4 = {}

---@param r number
---@param g number
---@param b number
---@param a number
---@return Color4
function Color4(r, g, b, a) end

---@class HitObject
---@field time number
---@field lane number
---@field sample string
---@field type string
---@field duration number
---@deprecated
---@field holdTime number (deprecated: Use 'duration' instead)
---@deprecated
---@field hitSound string (deprecated: Use 'sample' instead)
---@deprecated
---@field visualLane number (deprecated: No longer supported and always returns 1)
HitObject = {}

---@class Vector2
---@field x number
---@field y number
local __Vector2 = {}

---@param x number
---@param y number
---@return Vector2
function Vector2(x, y) end

---@class TimingPoint
---@field time number
---@field lane number
---@field bpm number
---@field signature number
---@field hideLines boolean
TimingPoint = {}