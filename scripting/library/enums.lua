﻿---@meta

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

---@param input AnchorName
---@return number
---@nodiscard
function Anchor(input) end

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

---@alias ParameterDefinitionType string
---| "string"
---| "int"
---| "float"