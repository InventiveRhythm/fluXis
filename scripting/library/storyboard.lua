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
local __StoryboardElement = {}

---@class StoryboardBox: StoryboardElement
local __StoryboardBox = {}

---@class StoryboardSprite: StoryboardElement
---@field texture string
local __StoryboardSprite = {}

---@class StoryboardText: StoryboardElement
---@field text string
---@field size number
local __StoryboardText = {}

---@class LuaMetadata
---@field title string
---@field artist string
---@field mapper string
---@field difficulty string
---@field background string
---@field cover string
local __Metadata = {}

---@type LuaMetadata
---@diagnostic disable-next-line: missing-fields
metadata = {};

---@type Vector2
---@diagnostic disable-next-line: missing-fields
screen = {};

---@param type AnimationType
---@param time number
---@param len number
---@param startval string
---@param endval string
---@param ease Easing
function __StoryboardElement:animate(type, time, len, startval, endval, ease) end

---@param key string
---@param fallback any
---@return any
---@nodiscard
function __StoryboardElement:param(key, fallback) end

---@return StoryboardBox
function StoryboardBox() end

---@return StoryboardSprite
function StoryboardSprite() end

---@return StoryboardText
function StoryboardText() end

---@param input string
---@return number
---@nodiscard
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
