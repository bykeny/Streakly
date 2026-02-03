variable "location" {
  type    = string
  default = "polandcentral"
}

variable "resource_group_name" {
  type    = string
  default = "rg-streakly"
}

variable "containerapp_name" {
  type    = string
  default = "streakly-app"
}

variable "environment_name" {
  type    = string
  default = "env-streakly"
}

variable "image_name" {
  type    = string
  default = "bykeny/habit-goal-tracker:latest"
}

variable "smtp_host" {
  type    = string
  default = "smtp.gmail.com"
}

variable "smtp_port" {
  type    = string
  default = "587"
}

variable "smtp_username" {
  type = string
}

variable "smtp_password" {
  type      = string
  sensitive = true
}

variable "smtp_from_email" {
  type    = string
  default = "streaklyapp@gmail.com"
}

variable "smtp_from_name" {
  type    = string
  default = "Streakly"
}

variable "smtp_use_ssl" {
  type    = string
  default = "true"
}