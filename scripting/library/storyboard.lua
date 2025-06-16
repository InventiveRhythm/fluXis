---@meta

---@class StoryboardElement
---@field layer number
---@field time number
---@field endtime number
---@field anchor number
---@field origin number
---@field x number
---@field y number
---@field z number
---@field blend boolean
---@field width number
---@field height number
---@field color number
local StoryboardElement = {}

---@type Vector2
---@diagnostic disable-next-line: missing-fields
screen = {};

---@param type AnimationType
---@param time number
---@param len number
---@param startval string
---@param endval string
---@param ease Easing
function StoryboardElement:animate(type, time, len, startval, endval, ease) end

---@return StoryboardElement
function StoryboardBox() end

---@return StoryboardElement
function StoryboardSprite() end

---@return StoryboardElement
function StoryboardText() end

---@param input string
---@return number
function Layer(input) end

---@param element StoryboardElement
function Add(element) end

---@alias AnimationType string
---| "MoveX"
---| "MoveY"
---| "Scale"
---| "ScaleVector"
---| "Width"
---| "Height"
---| "Rotate"
---| "Fade"
---| "Color"
