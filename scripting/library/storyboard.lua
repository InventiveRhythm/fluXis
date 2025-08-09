---@meta

---@type Vector2
---@diagnostic disable-next-line: missing-fields
screen = {}

---@param element StoryboardElement
function Add(element) end

---@param input string
---@return number
---@nodiscard
function Layer(input) end

---@return StoryboardBox
---@nodiscard
function StoryboardBox() end

---@return StoryboardSprite
---@nodiscard
function StoryboardSprite() end

---@return StoryboardText
---@nodiscard
function StoryboardText() end

---@class metadata
---@field title string
---@field artist string
---@field mapper string
---@field difficulty string
---@field background string
---@field cover string
metadata = {}

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

---@param type AnimationType
---@param time number
---@param len number
---@param startVal string
---@param endVal string
---@param ease Easing
function __StoryboardElement:animate(type, time, len, startVal, endVal, ease) end

---@param key string
---@param fallback any
---@return any
---@nodiscard
function __StoryboardElement:param(key, fallback) end

---@class StoryboardBox: StoryboardElement
local __StoryboardBox = {}

---@class StoryboardSprite: StoryboardElement
---@field texture string
local __StoryboardSprite = {}

---@class StoryboardText: StoryboardElement
---@field size number
---@field text string
local __StoryboardText = {}

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