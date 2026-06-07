---@meta

---@class BeatPulseEvent
---@field time number
---@field group string
---@field strength number
---@field zoomIn number How much of the length should be used to zoom in. (in %)
---@field interval number
BeatPulseEvent = {}

---@class ColorFadeEvent
---@field time number
---@field group string
---@field fadePrimary boolean
---@field primary Color4
---@field fadeSecondary boolean
---@field secondary Color4
---@field fadeMiddle boolean
---@field middle Color4
---@field duration number
---@field easing number
---@field playfieldIndex number
---@field playfieldSubIndex number
ColorFadeEvent = {}

---@class FlashEvent
---@field time number
---@field group string
---@field duration number
---@field inBackground boolean
---@field easing number
---@field startColor Color4
---@field startOpacity number
---@field endColor Color4
---@field endOpacity number
FlashEvent = {}

---@class LaneSwitchEvent
---@field time number
---@field group string
---@field count number
---@field duration number
---@field easing number
LaneSwitchEvent = {}

---@class LayerFadeEvent
---@field time number
---@field group string
---@field duration number
---@field alpha number
---@field easing number
---@field layer string
---@field playfieldIndex number
---@field playfieldSubIndex number
LayerFadeEvent = {}

---@class NoteEvent
---@field time number
---@field group string
---@field content string
NoteEvent = {}

---@class PulseEvent
---@field time number
---@field group string
---@field width number
---@field duration number
---@field inPercent number
---@field easing number
PulseEvent = {}

---@class ShaderEvent
---@field time number
---@field group string
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
---@field group string
---@field duration number
---@field magnitude number
ShakeEvent = {}

---@class HitObjectEaseEvent
---@field time number
---@field group string
---@field easing number
HitObjectEaseEvent = {}

---@class ScrollMultiplierEvent
---@field time number
---@field group string
---@field duration number
---@field multiplier number
---@field easing number
---@field groups string[]
ScrollMultiplierEvent = {}

---@class TimeOffsetEvent
---@field time number
---@field group string
---@field duration number
---@field useStartValue boolean
---@field startOffset number
---@field targetOffset number
---@field easing number
TimeOffsetEvent = {}

---@class PlayfieldMoveEvent
---@field time number
---@field group string
---@field offsetX number
---@field offsetY number
---@field offsetZ number
---@field duration number
---@field easing number
---@field playfieldIndex number
---@field playfieldSubIndex number
PlayfieldMoveEvent = {}

---@class PlayfieldRotateEvent
---@field time number
---@field group string
---@field roll number
---@field duration number
---@field easing number
---@field playfieldIndex number
---@field playfieldSubIndex number
PlayfieldRotateEvent = {}

---@class PlayfieldScaleEvent
---@field time number
---@field group string
---@field scaleX number
---@field scaleY number
---@field duration number
---@field easing number
---@field playfieldIndex number
---@field playfieldSubIndex number
PlayfieldScaleEvent = {}

---@class CameraMoveEvent
---@field time number
---@field group string
---@field x number
---@field y number
---@field duration number
---@field easing number
CameraMoveEvent = {}

---@class CameraRotateEvent
---@field time number
---@field group string
---@field roll number
---@field duration number
---@field easing number
CameraRotateEvent = {}

---@class CameraScaleEvent
---@field time number
---@field group string
---@field scale number
---@field duration number
---@field easing number
CameraScaleEvent = {}