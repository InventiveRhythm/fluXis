---@meta

---@class BeatPulseEvent
---@field time number
---@field strength number
---@field zoom number How much of the length should be used to zoom in. (in %)
---@field interval number
BeatPulseEvent = {}

---@class ColorFadeEvent
---@field time number
---@field fadePrimary boolean
---@field primary string
---@field fadeSecondary boolean
---@field secondary string
---@field fadeMiddle boolean
---@field middle string
---@field duration number
---@field ease number
---@field playfield number
---@field subfield number
ColorFadeEvent = {}

---@class FlashEvent
---@field time number
---@field duration number
---@field background boolean
---@field ease number
---@field startColor string
---@field startAlpha number
---@field endColor string
---@field endAlpha number
FlashEvent = {}

---@class LaneSwitchEvent
---@field time number
---@field count number
---@field speed number
---@field easing number
LaneSwitchEvent = {}

---@class LayerFadeEvent
---@field time number
---@field duration number
---@field alpha number
---@field ease number
---@field layer string
---@field playfield number
---@field subfield number
LayerFadeEvent = {}

---@class NoteEvent
---@field time number
---@field content string
NoteEvent = {}

---@class PulseEvent
---@field time number
---@field width number
---@field duration number
---@field inPercent number
---@field easing number
PulseEvent = {}

---@class ScriptEvent
---@field time number
---@field path string
---@field params string
ScriptEvent = {}

---@class ShaderEvent
---@field time number
---@field shader string
---@field duration number
---@field ease number
---@field useStart boolean
---@field startParams string
---@field endParams string
---@field params string
ShaderEvent = {}

---@class ShaderParameters
---@field strength number
---@field strength2 number
---@field strength3 number
ShaderParameters = {}

---@class ShakeEvent
---@field time number
---@field duration number
---@field magnitude number
ShakeEvent = {}