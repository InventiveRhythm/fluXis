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

---@class Vector2
---@field x number
---@field y number
local __Vector2 = {}

---@param x number
---@param y number
---@return Vector2
function Vector2(x, y) end

---@class HitObject
---@field time number
---@field lane number
---@field visualLane number the visual position of the note. (only applies to tick notes)
---@field holdTime number
---@field hitSound string
---@field group string
---@field hidden boolean
---@field type number
HitObject = {}

---@class HitSoundFade
---@field time number The time at which the volume change should start.
---@field hitSound string The sound to change the volume of.
---@field volume number The volume to fade to.
---@field duration number The duration of the fade.
---@field easing number The easing function to use for the fade.
HitSoundFade = {}

---@class ScrollVelocity
---@field time number
---@field group string
---@field multiplier number
---@field groups string[]
---@field laneMask boolean[]
ScrollVelocity = {}

---@class TimingPoint
---@field time number
---@field bpm number
---@field signature number
---@field hideLines boolean
TimingPoint = {}