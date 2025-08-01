import { NgModule } from '@angular/core';
import { Activity, Bell, ChevronDown, ChevronLeft, ChevronRight, CircleDashed, Clock, Clock10, Edit, IndianRupee, LayoutDashboard, List, Lock, LockOpen, LogInIcon, LucideAngularModule, Menu, MessageCircle, MoreVertical, Plus, Settings, TerminalSquare, Ticket, TrashIcon, UserRoundPen, Users, X } from 'lucide-angular';
import { Camera, User, LogIn, LogOut, Sun, Moon } from 'lucide-angular';

@NgModule({
    imports: [
        LucideAngularModule.pick({
            IndianRupee,
            Camera,
            User,
            LogIn,
            LogOut,
            Sun,
            Moon,
            Bell,
            Clock,
            LayoutDashboard,
            Settings,
            TerminalSquare,
            Ticket,
            Menu,
            Plus,
            MessageCircle,
            Activity,
            Clock10,
            ChevronLeft,
            ChevronRight,
            ChevronDown,
            LogInIcon,
            MoreVertical,
            X,
            Users,
            UserRoundPen,
            LockOpen,
            Lock,
            CircleDashed,
            Edit,
            List,
            TrashIcon,
        })
    ],
    exports: [LucideAngularModule]
})
export class LucideIconsModule {}