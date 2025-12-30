---@meta

---@class BeatPulseEvent
---@field time number
---@field strength number
---@field zoomIn number How much of the length should be used to zoom in. (in %)
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
---@field easing number
---@field playfieldIndex number
---@field playfieldSubIndex number
ColorFadeEvent = {}

---@class FlashEvent
---@field time number
---@field duration number
---@field inBackground boolean
---@field easing number
---@field startColor string
---@field startOpacity number
---@field endColor string
---@field endOpacity number
FlashEvent = {}

---@class LaneSwitchEvent
---@field time number
---@field count number
---@field duration number
---@field easing number
LaneSwitchEvent = {}

---@class LayerFadeEvent
---@field time number
---@field duration number
---@field alpha number
---@field easing number
---@field layer string
---@field playfieldIndex number
---@field playfieldSubIndex number
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
---@field scriptPath string
---@field parameters string
ScriptEvent = {}

---@class ShaderEvent
---@field time number
---@field shaderName string
---@field duration number
---@field easing number
---@field useStartParams boolean
---@field startParameters string
---@field endParameters string
---@field parameters string
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