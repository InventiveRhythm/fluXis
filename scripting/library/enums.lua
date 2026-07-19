---@meta

---@alias Easing string
---| "None"
---| "Out"
---| "In"
---| "InQuad"
---| "OutQuad"
---| "InOutQuad"
---| "InCubic"
---| "OutCubic"
---| "InOutCubic"
---| "InQuart"
---| "OutQuart"
---| "InOutQuart"
---| "InQuint"
---| "OutQuint"
---| "InOutQuint"
---| "InSine"
---| "OutSine"
---| "InOutSine"
---| "InExpo"
---| "OutExpo"
---| "InOutExpo"
---| "InCirc"
---| "OutCirc"
---| "InOutCirc"
---| "InElastic"
---| "OutElastic"
---| "OutElasticHalf"
---| "OutElasticQuarter"
---| "InOutElastic"
---| "InBack"
---| "OutBack"
---| "InOutBack"
---| "InBounce"
---| "OutBounce"
---| "InOutBounce"
---| "OutPow10"

---@param input Easing
---@return number
---@nodiscard
function Easing(input) end

---@alias HitObjectType string
---| "Normal"
---| "Tick"
---| "Landmine"

---@param input HitObjectType
---@return number
---@nodiscard
function HitObjectType(input) end

---@alias AnchorName string
---| "TopLeft"
---| "TopCentre"
---| "TopRight"
---| "CentreLeft"
---| "Centre"
---| "CentreRight"
---| "BottomLeft"
---| "BottomCentre"
---| "BottomRight"

---@param input AnchorName
---@return number
---@nodiscard
function Anchor(input) end

---@alias BlendMode string
---| "None"
---| "Inherit"
---| "Mix"
---| "LegacyDifference"
---| "Add"
---| "Subtract"
---| "Screen"
---| "Multiply"
---| "Premultiplied"
---| "Difference"

---@param input BlendMode
---@return number
---@nodiscard
function BlendMode(input) end

---@alias FFTBandType string
---| "Low"
---| "Mid"
---| "High"

---@alias EventType string
---| "BeatPulse"
---| "ColorFade"
---| "Flash"
---| "LaneSwitch"
---| "LayerFade"
---| "Pulse"
---| "Shader"
---| "Shake"
---| "HitObjectEase"
---| "ScrollMultiplier"
---| "TimeOffset"
---| "PlayfieldMove"
---| "PlayfieldRotate"
---| "PlayfieldScale"
---| "Loop"
---| "CameraMove"
---| "CameraRotate"
---| "CameraScale"

---@alias ParameterDefinitionType string
---| "string"
---| "int"
---| "float"
---| "boolean"