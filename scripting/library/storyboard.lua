---@meta

---@type Vector2
---@diagnostic disable-next-line: missing-fields
screen = {}

---@param element StoryboardElement
function Add(element) end

---@return StoryboardBox
---@nodiscard
function StoryboardBox() end

---@return StoryboardSprite
---@nodiscard
function StoryboardSprite() end

---@return StoryboardText
---@nodiscard
function StoryboardText() end

---@return StoryboardCircle
---@nodiscard
function StoryboardCircle() end

---@return StoryboardOutlineCircle
---@nodiscard
function StoryboardOutlineCircle() end

---@param str SkinSprite
---@return StoryboardSkinSprite
---@nodiscard
function StoryboardSkinSprite(str) end

---@class settings
---@field scrollspeed number
---@field upscroll boolean
settings = {}

---@class metadata
---@field title string the non-romanized title of the current map
---@field artist string the non-romanized artist of the current map
---@field mapper string
---@field difficulty string difficulty name of the current map
---@field background string relative path to the background image
---@field cover string relative path to the cover image
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

---applies a new animation to this element
---@param type AnimationType the type of animation
---@param time number when this animation starts in ms (absolute from map start)
---@param len number the total length of this animation in ms
---@param startVal string the value this animation starts with (input based on type)
---@param endVal string the value this animation ends with (input based on type)
---@param ease Easing the easing function used for this animation
function __StoryboardElement:animate(type, time, len, startVal, endVal, ease) end

---@param key string
---@param fallback any
---@return any
---@nodiscard
function __StoryboardElement:param(key, fallback) end

---@class StoryboardBox: StoryboardElement
local __StoryboardBox = {}

---@class StoryboardCircle: StoryboardElement
local __StoryboardCircle = {}

---@class StoryboardOutlineCircle: StoryboardElement
local __StoryboardOutlineCircle = {}

---@class StoryboardSkinSprite: StoryboardElement
---@field sprite number
---@field lane number
---@field keycount number
local __StoryboardSkinSprite = {}

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
---| "Border"

---@alias SkinSprite string
---| "HitObject"
---| "LongNoteStart"
---| "LongNoteBody"
---| "LongNoteEnd"
---| "TickNote"
---| "TickNoteSmall"
---| "Receptor"
---| "StageBackground"
---| "StageBackgroundTop"
---| "StageBackgroundBottom"
---| "StageLeftTop"
---| "StageLeft"
---| "StageLeftBottom"
---| "StageRightTop"
---| "StageRight"
---| "StageRightBottom"

---@alias LayerName string
---| "Background"
---| "Foreground"
---| "Overlay"

---@param input LayerName
---@return number
---@nodiscard
function Layer(input) end